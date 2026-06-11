using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Infrastructure.Persistence.Entity;

/// <summary>Entidad de persistencia (EF Core) del movimiento. Tabla <c>movimiento</c>.</summary>
public class MovimientoEntity
{
    public long Id { get; set; }
    public DateTime Fecha { get; set; }
    public TipoMovimiento TipoMovimiento { get; set; }
    public decimal Valor { get; set; }
    public decimal Saldo { get; set; }
    public long CuentaId { get; set; }
    public string? Descripcion { get; set; }
}
