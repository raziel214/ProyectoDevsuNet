using Devsu.Cuentas.Application.Port.In;
using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Devsu.Cuentas.Application.Service;

/// <summary>
/// Mantiene la réplica local de clientes a partir de los eventos de ms-clientes.
/// Idempotente (upsert), de modo que reprocesar un mensaje no genera
/// inconsistencias.
/// </summary>
public sealed class ClienteSyncService(
    IClienteRefRepositoryPort clienteRefRepository,
    ILogger<ClienteSyncService> logger)
    : IClienteSyncUseCase
{
    public async Task SincronizarAsync(ClienteRef clienteRef, CancellationToken ct = default)
    {
        await clienteRefRepository.GuardarAsync(clienteRef, ct);
        logger.LogInformation("Replica sincronizada clienteId={ClienteId} estado={Estado}",
            clienteRef.ClienteId, clienteRef.Estado);
    }

    public async Task DesactivarAsync(string clienteId, CancellationToken ct = default)
    {
        var existente = await clienteRefRepository.BuscarPorClienteIdAsync(clienteId, ct);
        if (existente is null)
        {
            // Nada que inhabilitar; idempotente.
            return;
        }

        var inhabilitado = new ClienteRef
        {
            ClienteId = existente.ClienteId,
            Nombre = existente.Nombre,
            Identificacion = existente.Identificacion,
            Estado = false,
            ActualizadoEn = DateTime.UtcNow
        };

        await clienteRefRepository.GuardarAsync(inhabilitado, ct);
        logger.LogInformation("Replica inhabilitada (DELETED) clienteId={ClienteId}", clienteId);
    }
}
