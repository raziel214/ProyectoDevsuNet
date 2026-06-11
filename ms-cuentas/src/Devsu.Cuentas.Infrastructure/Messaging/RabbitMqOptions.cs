namespace Devsu.Cuentas.Infrastructure.Messaging;

/// <summary>
/// Configuración de RabbitMQ del lado consumidor (sección <c>RabbitMq</c>).
/// Topología según el contrato async: exchange topic, cola durable con binding
/// <c>cliente.*</c> y dead-letter tras agotar reintentos.
/// </summary>
public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";

    /// <summary>Exchange de donde provienen los eventos de cliente.</summary>
    public string Exchange { get; set; } = "devsu.clientes.exchange";

    /// <summary>Cola durable del consumidor en ms-cuentas.</summary>
    public string Queue { get; set; } = "devsu.cuentas.cliente-sync.queue";

    /// <summary>Patrón de routing keys a escuchar.</summary>
    public string RoutingKey { get; set; } = "cliente.*";

    /// <summary>Cola dead-letter para mensajes fallidos.</summary>
    public string DeadLetterQueue { get; set; } = "devsu.cuentas.cliente-sync.dlq";

    /// <summary>Exchange dead-letter (default, por routing directo a la DLQ).</summary>
    public string DeadLetterExchange { get; set; } = "devsu.cuentas.cliente-sync.dlx";
}
