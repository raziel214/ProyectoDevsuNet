using Devsu.Cuentas.Application.Port.In;
using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Domain.Exceptions;
using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Application.Service;

/// <summary>
/// Servicio de aplicación de Movimientos (F1/F2/F3).
///
/// <para>Registrar un movimiento actualiza el saldo de la cuenta (la invariante
/// "saldo no negativo" la valida el dominio, F3) y crea el asiento del ledger,
/// ambos de forma <b>atómica</b> (IUnitOfWork). El ledger es inmutable: solo la
/// descripción es editable.</para>
/// </summary>
public sealed class MovimientoService(
    ICuentaRepositoryPort cuentaRepository,
    IMovimientoRepositoryPort movimientoRepository,
    IUnitOfWork unitOfWork)
    : IRegistrarMovimientoUseCase,
      IConsultarMovimientoUseCase,
      IActualizarMovimientoUseCase
{
    public async Task<Movimiento> RegistrarAsync(RegistrarMovimientoCommand command, CancellationToken ct = default)
    {
        var cuenta = await cuentaRepository.BuscarPorNumeroCuentaAsync(command.NumeroCuenta, ct)
                     ?? throw CuentaNoEncontradaException.PorNumero(command.NumeroCuenta);

        // El dominio aplica el movimiento y valida el saldo (lanza 422 si no alcanza).
        var cuentaActualizada = cuenta.AplicarMovimiento(command.Valor);

        var movimiento = new Movimiento
        {
            Fecha = DateTime.UtcNow,
            TipoMovimiento = Movimiento.TipoDe(command.Valor),
            Valor = command.Valor,
            Saldo = cuentaActualizada.SaldoDisponible,
            CuentaId = cuenta.Id!.Value
        };

        // Atómico: actualizar saldo + crear asiento se confirman juntos.
        Movimiento? guardado = null;
        await unitOfWork.ExecuteInTransactionAsync(async c =>
        {
            await cuentaRepository.GuardarAsync(cuentaActualizada, c);
            guardado = await movimientoRepository.GuardarAsync(movimiento, c);
        }, ct);

        return guardado!;
    }

    public async Task<Movimiento> BuscarPorIdAsync(long id, CancellationToken ct = default)
        => await movimientoRepository.BuscarPorIdAsync(id, ct)
           ?? throw new MovimientoNoEncontradoException(id);

    public async Task<IReadOnlyList<Movimiento>> ListarAsync(string? numeroCuenta = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(numeroCuenta))
            return await movimientoRepository.ListarAsync(ct);

        var cuenta = await cuentaRepository.BuscarPorNumeroCuentaAsync(numeroCuenta, ct);
        return cuenta is null
            ? []
            : await movimientoRepository.ListarPorCuentaIdAsync(cuenta.Id!.Value, ct);
    }

    public async Task<Movimiento> ActualizarDescripcionAsync(long id, string? descripcion, CancellationToken ct = default)
    {
        var movimiento = await movimientoRepository.BuscarPorIdAsync(id, ct)
                         ?? throw new MovimientoNoEncontradoException(id);

        return await movimientoRepository.GuardarAsync(movimiento.ConDescripcion(descripcion), ct);
    }
}
