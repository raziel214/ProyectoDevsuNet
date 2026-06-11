using Devsu.Cuentas.Domain.Exceptions;

namespace Devsu.Cuentas.Domain.Model;

/// <summary>
/// Agregado raíz: una Cuenta bancaria.
///
/// <para>Objeto de dominio puro (sin EF Core/ASP.NET), inmutable
/// (propiedades <c>init</c>-only). Encapsula la invariante de negocio del saldo:
/// un retiro no puede dejar el saldo disponible en negativo (F3).</para>
/// </summary>
public sealed class Cuenta
{
    public long? Id { get; init; }
    public required string NumeroCuenta { get; init; }
    public required TipoCuenta TipoCuenta { get; init; }
    public required decimal SaldoInicial { get; init; }
    public required decimal SaldoDisponible { get; init; }
    public required bool Estado { get; init; }

    /// <summary>clienteId dueño de la cuenta (referencia a la réplica local).</summary>
    public required string ClienteId { get; init; }

    /// <summary>
    /// Aplica un movimiento (valor con signo: + depósito, − retiro) y devuelve la
    /// cuenta con el saldo actualizado. Lanza <see cref="SaldoNoDisponibleException"/>
    /// si el retiro deja el saldo en negativo (F3).
    /// </summary>
    public Cuenta AplicarMovimiento(decimal valor)
    {
        var nuevoSaldo = SaldoDisponible + valor;
        if (nuevoSaldo < 0)
            throw new SaldoNoDisponibleException();
        return Copia(saldoDisponible: nuevoSaldo);
    }

    /// <summary>Devuelve la cuenta con el estado cambiado (operación sensible).</summary>
    public Cuenta CambiarEstado(bool estado) => Copia(estado: estado);

    /// <summary>Devuelve la cuenta con el tipo cambiado.</summary>
    public Cuenta CambiarTipo(TipoCuenta tipoCuenta) => Copia(tipoCuenta: tipoCuenta);

    private Cuenta Copia(TipoCuenta? tipoCuenta = null, decimal? saldoDisponible = null, bool? estado = null) => new()
    {
        Id = Id,
        NumeroCuenta = NumeroCuenta,
        TipoCuenta = tipoCuenta ?? TipoCuenta,
        SaldoInicial = SaldoInicial,
        SaldoDisponible = saldoDisponible ?? SaldoDisponible,
        Estado = estado ?? Estado,
        ClienteId = ClienteId
    };
}
