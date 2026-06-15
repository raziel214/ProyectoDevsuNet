namespace Devsu.Cuentas.Infrastructure.Persistence.Entity;

/// <summary>
/// Entidad de persistencia (EF Core) de la réplica local de cliente.
/// Tabla <c>cliente_ref</c> (read-model). La clave primaria es el clienteId.
/// </summary>
public class ClienteRefEntity
{
    public string ClienteId { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Identificacion { get; set; }
    public bool Estado { get; set; }
    public DateTime ActualizadoEn { get; set; }
}
