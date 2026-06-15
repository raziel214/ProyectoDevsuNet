using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Application.Port.In;

/// <summary>
/// Caso de uso: registrar un movimiento (F2/F3). Actualiza el saldo de la cuenta
/// y crea el asiento; devuelve 422 si el retiro supera el saldo disponible.
/// </summary>
public interface IRegistrarMovimientoUseCase
{
    Task<Movimiento> RegistrarAsync(RegistrarMovimientoCommand command, CancellationToken ct = default);
}

/// <summary>Caso de uso: consultar movimientos (F1).</summary>
public interface IConsultarMovimientoUseCase
{
    Task<Movimiento> BuscarPorIdAsync(long id, CancellationToken ct = default);

    /// <summary>Lista todos los movimientos, o los de una cuenta si se indica numeroCuenta.</summary>
    Task<IReadOnlyList<Movimiento>> ListarAsync(string? numeroCuenta = null, CancellationToken ct = default);
}

/// <summary>
/// Caso de uso: editar la descripción (nota) de un movimiento. El ledger es
/// inmutable: solo la descripción es editable.
/// </summary>
public interface IActualizarMovimientoUseCase
{
    Task<Movimiento> ActualizarDescripcionAsync(long id, string? descripcion, CancellationToken ct = default);
}
