namespace Devsu.Clientes.Infrastructure.Messaging;

/// <summary>
/// Configuración de la conexión y topología de RabbitMQ (sección <c>RabbitMq</c>).
/// Los valores provienen de la configuración (appsettings.json) y se pueden
/// sobrescribir por variables de entorno en el deploy. La clase no fija valores
/// de entorno (imagen inmutable + config externa).
/// </summary>
public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string VirtualHost { get; set; } = default!;

    /// <summary>Topic exchange donde se publican los eventos de cliente.</summary>
    public string Exchange { get; set; } = default!;
}
