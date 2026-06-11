namespace Devsu.Cuentas.Domain.Model;

/// <summary>
/// Réplica local de un Cliente (read-model / CQRS-lite).
///
/// <para>NO es la fuente de verdad: se sincroniza de forma asíncrona desde
/// ms-clientes vía eventos RabbitMQ (upsert idempotente por <see cref="ClienteId"/>).
/// La usa el reporte de estado de cuenta (F4) para resolver el nombre del cliente
/// y validar la existencia del cliente al crear una cuenta.</para>
/// </summary>
public sealed class ClienteRef
{
    public required string ClienteId { get; init; }
    public required string Nombre { get; init; }
    public string? Identificacion { get; init; }
    public required bool Estado { get; init; }

    /// <summary>Momento de la última sincronización.</summary>
    public DateTime ActualizadoEn { get; init; }
}
