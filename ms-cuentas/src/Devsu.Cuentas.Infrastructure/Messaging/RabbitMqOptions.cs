namespace Devsu.Cuentas.Infrastructure.Messaging;

/// <summary>
/// Configuración de RabbitMQ del lado consumidor (sección <c>RabbitMq</c>).
/// Los valores provienen de la configuración (appsettings.json) y se pueden
/// sobrescribir por variables de entorno en el deploy (imagen inmutable + config
/// externa). La clase no fija valores de entorno.
/// </summary>
public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string VirtualHost { get; set; } = default!;

    /// <summary>Exchange de donde provienen los eventos de cliente.</summary>
    public string Exchange { get; set; } = default!;

    /// <summary>Cola durable del consumidor en ms-cuentas.</summary>
    public string Queue { get; set; } = default!;

    /// <summary>Patrón de routing keys a escuchar.</summary>
    public string RoutingKey { get; set; } = default!;

    /// <summary>Cola dead-letter para mensajes fallidos.</summary>
    public string DeadLetterQueue { get; set; } = default!;

    /// <summary>Exchange dead-letter.</summary>
    public string DeadLetterExchange { get; set; } = default!;
}
