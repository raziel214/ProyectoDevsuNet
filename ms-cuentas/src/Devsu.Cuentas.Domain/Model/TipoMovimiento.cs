namespace Devsu.Cuentas.Domain.Model;

/// <summary>
/// Tipo de movimiento, derivado del signo del valor: positivo = depósito,
/// negativo = retiro. Value object del dominio.
/// </summary>
public enum TipoMovimiento
{
    DEPOSITO,
    RETIRO
}
