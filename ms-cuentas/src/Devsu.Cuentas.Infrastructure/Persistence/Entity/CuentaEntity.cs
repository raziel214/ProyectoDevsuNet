using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Infrastructure.Persistence.Entity;

/// <summary>Entidad de persistencia (EF Core) de la cuenta. Tabla <c>cuenta</c>.</summary>
public class CuentaEntity
{
    public long Id { get; set; }
    public string NumeroCuenta { get; set; } = null!;
    public TipoCuenta TipoCuenta { get; set; }
    public decimal SaldoInicial { get; set; }
    public decimal SaldoDisponible { get; set; }
    public bool Estado { get; set; }
    public string ClienteId { get; set; } = null!;
}
