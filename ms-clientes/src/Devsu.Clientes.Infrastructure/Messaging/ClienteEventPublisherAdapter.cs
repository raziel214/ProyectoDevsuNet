using System.Text.Json;
using Devsu.Clientes.Application.Port.Out;
using Devsu.Clientes.Domain.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Devsu.Clientes.Infrastructure.Messaging;

/// <summary>
/// Adaptador de salida que publica los <see cref="ClienteEvent"/> en RabbitMQ,
/// implementando <see cref="IClienteEventPublisherPort"/>.
///
/// <para>Traduce el dominio (<see cref="Cliente"/>) al evento de mensajería y lo
/// envía al topic exchange con la routing key correspondiente. Usa la API
/// async-first de RabbitMQ.Client v7.</para>
///
/// <para><b>Resiliencia:</b> la conexión se crea de forma perezosa y los fallos
/// de publicación se registran sin propagar. Así, si el broker está caído, la
/// operación de negocio (ya persistida en BD) no falla; solo se pierde/posterga
/// la notificación. Es un singleton que mantiene una conexión y abre un canal por
/// publicación (los canales no son thread-safe).</para>
/// </summary>
public sealed class ClienteEventPublisherAdapter : IClienteEventPublisherPort, IAsyncDisposable
{
    private const string RkCreated = "cliente.created";
    private const string RkUpdated = "cliente.updated";
    private const string RkDeleted = "cliente.deleted";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RabbitMqOptions _options;
    private readonly ILogger<ClienteEventPublisherAdapter> _logger;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;

    public ClienteEventPublisherAdapter(
        IOptions<RabbitMqOptions> options,
        ILogger<ClienteEventPublisherAdapter> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task PublicarCreadoAsync(Cliente cliente, CancellationToken ct = default)
        => PublicarAsync("CREATED", RkCreated, ToEvent("CREATED", cliente), cliente.ClienteId, ct);

    public Task PublicarActualizadoAsync(Cliente cliente, CancellationToken ct = default)
        => PublicarAsync("UPDATED", RkUpdated, ToEvent("UPDATED", cliente), cliente.ClienteId, ct);

    public Task PublicarEliminadoAsync(string clienteId, CancellationToken ct = default)
    {
        var evt = new ClienteEvent("DELETED", clienteId, null, null, null, Ahora());
        return PublicarAsync("DELETED", RkDeleted, evt, clienteId, ct);
    }

    private async Task PublicarAsync(string tipo, string routingKey, ClienteEvent evt, string clienteId, CancellationToken ct)
    {
        try
        {
            var channel = await AbrirCanalAsync(ct);
            await using (channel)
            {
                await channel.ExchangeDeclareAsync(
                    exchange: _options.Exchange,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: ct);

                var body = JsonSerializer.SerializeToUtf8Bytes(evt, JsonOptions);
                await channel.BasicPublishAsync(
                    exchange: _options.Exchange,
                    routingKey: routingKey,
                    body: body,
                    cancellationToken: ct);
            }

            _logger.LogInformation("Publicado ClienteEvent {Tipo} clienteId={ClienteId}", tipo, clienteId);
        }
        catch (Exception ex)
        {
            // No propagar: la operacion de negocio ya esta confirmada en BD.
            _logger.LogWarning(ex,
                "No se pudo publicar ClienteEvent {Tipo} clienteId={ClienteId}", tipo, clienteId);
        }
    }

    private async Task<IChannel> AbrirCanalAsync(CancellationToken ct)
    {
        var connection = await ObtenerConexionAsync(ct);
        return await connection.CreateChannelAsync(cancellationToken: ct);
    }

    private async Task<IConnection> ObtenerConexionAsync(CancellationToken ct)
    {
        if (_connection is { IsOpen: true })
            return _connection;

        await _connectionLock.WaitAsync(ct);
        try
        {
            if (_connection is { IsOpen: true })
                return _connection;

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost
            };
            _connection = await factory.CreateConnectionAsync(ct);
            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private static ClienteEvent ToEvent(string tipo, Cliente c)
        => new(tipo, c.ClienteId, c.Nombre, c.Identificacion, c.Estado, Ahora());

    private static string Ahora() => DateTimeOffset.UtcNow.ToString("O");

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
        _connectionLock.Dispose();
    }
}
