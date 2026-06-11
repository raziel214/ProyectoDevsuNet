using System.Text.Json;
using Devsu.Cuentas.Application.Port.In;
using Devsu.Cuentas.Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Devsu.Cuentas.Infrastructure.Messaging;

/// <summary>
/// Adaptador de entrada (driving) que consume los eventos de cliente de RabbitMQ
/// y sincroniza la réplica local vía <see cref="IClienteSyncUseCase"/>.
///
/// <para>Declara la topología (exchange topic, cola durable con binding
/// <c>cliente.*</c> y dead-letter). Procesa con hasta 3 intentos; al agotarlos el
/// mensaje va a la DLQ. Idempotente (el upsert tolera reprocesos).</para>
/// </summary>
public sealed class ClienteSyncConsumer(
    IOptions<RabbitMqOptions> options,
    IServiceScopeFactory scopeFactory,
    ILogger<ClienteSyncConsumer> logger) : BackgroundService
{
    private const int MaxIntentos = 3;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly RabbitMqOptions _options = options.Value;
    private IConnection? _connection;
    private IChannel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConectarConReintentosAsync(stoppingToken);
        await DeclararTopologiaAsync(stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel!);
        consumer.ReceivedAsync += OnMessageAsync;
        await _channel!.BasicConsumeAsync(_options.Queue, autoAck: false, consumer, stoppingToken);

        logger.LogInformation("Consumidor escuchando {Queue} (binding {Rk})", _options.Queue, _options.RoutingKey);

        try { await Task.Delay(Timeout.Infinite, stoppingToken); }
        catch (OperationCanceledException) { /* shutdown */ }
    }

    private async Task OnMessageAsync(object sender, BasicDeliverEventArgs ea)
    {
        ClienteEvent? evt;
        try
        {
            evt = JsonSerializer.Deserialize<ClienteEvent>(ea.Body.Span, JsonOptions);
        }
        catch (Exception ex)
        {
            // Mensaje ilegible: nunca va a procesarse bien -> directo a la DLQ.
            logger.LogError(ex, "Mensaje ilegible; se envia a la DLQ.");
            await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            return;
        }

        if (evt is null || string.IsNullOrEmpty(evt.ClienteId))
        {
            logger.LogWarning("Evento vacio o sin clienteId; se envia a la DLQ.");
            await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            return;
        }

        for (var intento = 1; intento <= MaxIntentos; intento++)
        {
            try
            {
                await ProcesarAsync(evt, CancellationToken.None);
                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Fallo procesando evento {Tipo} clienteId={ClienteId} (intento {Intento}/{Max})",
                    evt.EventType, evt.ClienteId, intento, MaxIntentos);
                if (intento < MaxIntentos)
                    await Task.Delay(TimeSpan.FromMilliseconds(500 * intento));
            }
        }

        logger.LogError("Agotados los reintentos para clienteId={ClienteId}; se envia a la DLQ.", evt.ClienteId);
        await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
    }

    private async Task ProcesarAsync(ClienteEvent evt, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var sync = scope.ServiceProvider.GetRequiredService<IClienteSyncUseCase>();

        switch (evt.EventType?.ToUpperInvariant())
        {
            case "CREATED":
            case "UPDATED":
                await sync.SincronizarAsync(new ClienteRef
                {
                    ClienteId = evt.ClienteId,
                    Nombre = evt.Nombre ?? string.Empty,
                    Identificacion = evt.Identificacion,
                    Estado = evt.Estado ?? true,
                    ActualizadoEn = DateTime.UtcNow
                }, ct);
                break;

            case "DELETED":
                await sync.DesactivarAsync(evt.ClienteId, ct);
                break;

            default:
                logger.LogWarning("eventType desconocido: {Tipo}", evt.EventType);
                break;
        }
    }

    private async Task ConectarConReintentosAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };

        for (var intento = 1; !ct.IsCancellationRequested; intento++)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync(ct);
                _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
                logger.LogInformation("Conectado a RabbitMQ en {Host}:{Port}", _options.Host, _options.Port);
                return;
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                var espera = TimeSpan.FromSeconds(Math.Min(30, 2 * intento));
                logger.LogWarning(ex, "RabbitMQ no disponible (intento {Intento}); reintento en {Espera}s",
                    intento, espera.TotalSeconds);
                await Task.Delay(espera, ct);
            }
        }
    }

    private async Task DeclararTopologiaAsync(CancellationToken ct)
    {
        // Dead-letter: exchange fanout + cola, para mensajes fallidos.
        await _channel!.ExchangeDeclareAsync(_options.DeadLetterExchange, ExchangeType.Fanout,
            durable: true, autoDelete: false, cancellationToken: ct);
        await _channel.QueueDeclareAsync(_options.DeadLetterQueue, durable: true, exclusive: false,
            autoDelete: false, cancellationToken: ct);
        await _channel.QueueBindAsync(_options.DeadLetterQueue, _options.DeadLetterExchange, routingKey: string.Empty,
            cancellationToken: ct);

        // Exchange de dominio (topic) — declaración idempotente, igual que el productor.
        await _channel.ExchangeDeclareAsync(_options.Exchange, ExchangeType.Topic,
            durable: true, autoDelete: false, cancellationToken: ct);

        // Cola del consumidor con dead-letter configurado, y binding cliente.*
        var args = new Dictionary<string, object?> { ["x-dead-letter-exchange"] = _options.DeadLetterExchange };
        await _channel.QueueDeclareAsync(_options.Queue, durable: true, exclusive: false,
            autoDelete: false, arguments: args, cancellationToken: ct);
        await _channel.QueueBindAsync(_options.Queue, _options.Exchange, _options.RoutingKey, cancellationToken: ct);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null) await _channel.CloseAsync(cancellationToken);
        if (_connection is not null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
