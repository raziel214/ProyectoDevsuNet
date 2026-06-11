using Devsu.Cuentas.Application.Port.In;
using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Domain.Exceptions;
using Devsu.Cuentas.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Devsu.Cuentas.Application.Service;

/// <summary>
/// Servicio de aplicación de Cuentas (F1). Crear valida que el cliente exista en
/// la réplica local y que el número de cuenta sea único. El cambio de estado es
/// una operación sensible y auditada.
/// </summary>
public sealed class CuentaService(
    ICuentaRepositoryPort cuentaRepository,
    IClienteRefRepositoryPort clienteRefRepository,
    ILogger<CuentaService> logger)
    : ICrearCuentaUseCase,
      IConsultarCuentaUseCase,
      IActualizarCuentaUseCase,
      ICambiarEstadoCuentaUseCase
{
    public async Task<Cuenta> CrearAsync(CrearCuentaCommand command, CancellationToken ct = default)
    {
        // El cliente debe existir en la réplica local (sincronizada por eventos).
        _ = await clienteRefRepository.BuscarPorClienteIdAsync(command.ClienteId, ct)
            ?? throw new ClienteNoEncontradoException(command.ClienteId);

        if (await cuentaRepository.ExistePorNumeroCuentaAsync(command.NumeroCuenta, ct))
            throw new NumeroCuentaDuplicadoException(command.NumeroCuenta);

        var cuenta = new Cuenta
        {
            NumeroCuenta = command.NumeroCuenta,
            TipoCuenta = command.TipoCuenta,
            SaldoInicial = command.SaldoInicial,
            SaldoDisponible = command.SaldoInicial, // al crear, disponible = inicial
            Estado = command.Estado,
            ClienteId = command.ClienteId
        };

        return await cuentaRepository.GuardarAsync(cuenta, ct);
    }

    public async Task<Cuenta> BuscarPorIdAsync(long id, CancellationToken ct = default)
        => await cuentaRepository.BuscarPorIdAsync(id, ct)
           ?? throw new CuentaNoEncontradaException(id);

    public Task<IReadOnlyList<Cuenta>> ListarAsync(string? clienteId = null, CancellationToken ct = default)
        => string.IsNullOrWhiteSpace(clienteId)
            ? cuentaRepository.ListarAsync(ct)
            : cuentaRepository.ListarPorClienteIdAsync(clienteId, ct);

    public async Task<Cuenta> ActualizarAsync(long id, ActualizarCuentaCommand command, CancellationToken ct = default)
    {
        var cuenta = await cuentaRepository.BuscarPorIdAsync(id, ct)
                     ?? throw new CuentaNoEncontradaException(id);

        return await cuentaRepository.GuardarAsync(cuenta.CambiarTipo(command.TipoCuenta), ct);
    }

    public async Task<Cuenta> CambiarEstadoAsync(long id, bool estado, CancellationToken ct = default)
    {
        var cuenta = await cuentaRepository.BuscarPorIdAsync(id, ct)
                     ?? throw new CuentaNoEncontradaException(id);

        var guardada = await cuentaRepository.GuardarAsync(cuenta.CambiarEstado(estado), ct);
        logger.LogInformation("AUDIT cambio de estado cuenta numeroCuenta={Numero} estado={Estado}",
            guardada.NumeroCuenta, estado);
        return guardada;
    }
}
