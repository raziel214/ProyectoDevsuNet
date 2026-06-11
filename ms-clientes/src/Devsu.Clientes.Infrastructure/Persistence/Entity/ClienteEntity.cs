using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Infrastructure.Persistence.Entity;

/// <summary>
/// Entidad de persistencia (EF Core) del cliente. Mapea a la tabla <c>cliente</c>,
/// que incluye tanto las columnas de los datos personales (en el dominio, la base
/// <c>Persona</c>) como las propias del cliente, en una sola tabla.
///
/// <para>Modelo de persistencia (infraestructura), separado del agregado de
/// dominio <see cref="Cliente"/>. Es mutable y conoce EF Core. La conversión
/// entre ambos la realiza el <c>ClientePersistenceMapper</c>; así el dominio se
/// mantiene libre de EF Core (Hexagonal estricto).</para>
/// </summary>
public class ClienteEntity
{
    public long Id { get; set; }
    public string Nombre { get; set; } = null!;
    public Genero? Genero { get; set; }
    public int? Edad { get; set; }
    public string Identificacion { get; set; } = null!;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string ClienteId { get; set; } = null!;

    /// <summary>Contraseña hasheada con BCrypt (nunca en texto plano).</summary>
    public string Contrasena { get; set; } = null!;
    public bool Estado { get; set; }
}
