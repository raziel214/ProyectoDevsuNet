namespace Devsu.Clientes.Infrastructure.Messaging;

/// <summary>
/// Configuración de la conexión y topología de RabbitMQ (sección <c>RabbitMq</c>
/// de la configuración). Los eventos de cliente se publican en un topic exchange.
/// </summary>
public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";

    /// <summary>Topic exchange donde se publican los eventos de cliente.</summary>
    public string Exchange { get; set; } = "devsu.clientes.exchange";
}
