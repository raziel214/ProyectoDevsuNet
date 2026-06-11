using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Application.Port.In;

/// <summary>
/// Caso de uso: sincronizar la réplica local de clientes a partir de los eventos
/// de ms-clientes (consumidos por RabbitMQ). Idempotente.
/// </summary>
public interface IClienteSyncUseCase
{
    /// <summary>CREATED / UPDATED → upsert de la réplica por clienteId.</summary>
    Task SincronizarAsync(ClienteRef clienteRef, CancellationToken ct = default);

    /// <summary>
    /// DELETED → inhabilita la réplica (estado=false). No se elimina la fila para
    /// conservar el nombre en reportes históricos (consistencia bancaria).
    /// </summary>
    Task DesactivarAsync(string clienteId, CancellationToken ct = default);
}
