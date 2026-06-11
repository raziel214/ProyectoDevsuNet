using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Application.Port.Out;

/// <summary>
/// Puerto de salida para la réplica local de clientes (<see cref="ClienteRef"/>).
/// Read-model: se escribe al consumir eventos (upsert idempotente) y se lee en el
/// reporte (F4) y al validar la creación de cuentas.
/// </summary>
public interface IClienteRefRepositoryPort
{
    /// <summary>Inserta o actualiza la réplica del cliente (upsert por clienteId).</summary>
    Task GuardarAsync(ClienteRef clienteRef, CancellationToken ct = default);

    Task<ClienteRef?> BuscarPorClienteIdAsync(string clienteId, CancellationToken ct = default);

    Task EliminarPorClienteIdAsync(string clienteId, CancellationToken ct = default);
}
