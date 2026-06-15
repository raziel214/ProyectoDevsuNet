namespace Devsu.Cuentas.Domain.Model;

/// <summary>
/// Movimiento de una cuenta (asiento del ledger).
///
/// <para><b>Ledger inmutable:</b> una vez registrado, sus datos contables (fecha,
/// tipo, valor, saldo resultante) no cambian. El único campo editable es la
/// <see cref="Descripcion"/> (metadato/nota). Objeto de dominio puro e inmutable.</para>
/// </summary>
public sealed class Movimiento
{
    public long? Id { get; init; }
    public required DateTime Fecha { get; init; }
    public required TipoMovimiento TipoMovimiento { get; init; }

    /// <summary>Valor con signo: positivo = depósito, negativo = retiro.</summary>
    public required decimal Valor { get; init; }

    /// <summary>Saldo disponible resultante tras aplicar el movimiento.</summary>
    public required decimal Saldo { get; init; }

    public required long CuentaId { get; init; }

    /// <summary>Único campo mutable: nota/metadato del movimiento.</summary>
    public string? Descripcion { get; init; }

    /// <summary>Deriva el tipo a partir del signo del valor.</summary>
    public static TipoMovimiento TipoDe(decimal valor)
        => valor >= 0 ? TipoMovimiento.DEPOSITO : TipoMovimiento.RETIRO;

    /// <summary>Devuelve el movimiento con la descripción cambiada (único campo editable).</summary>
    public Movimiento ConDescripcion(string? descripcion) => new()
    {
        Id = Id,
        Fecha = Fecha,
        TipoMovimiento = TipoMovimiento,
        Valor = Valor,
        Saldo = Saldo,
        CuentaId = CuentaId,
        Descripcion = descripcion
    };
}
