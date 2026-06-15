namespace Devsu.Clientes.Domain.Model;

/// <summary>
/// Entidad base del dominio que representa los datos personales comunes.
///
/// <para>Es la superclase de <see cref="Cliente"/> (relación de herencia exigida
/// por el dominio: "un cliente es un tipo de persona"). Se declara
/// <c>abstract</c> porque una Persona nunca existe por sí sola en este contexto:
/// siempre es un Cliente.</para>
///
/// <para><b>Pureza hexagonal:</b> es un objeto de dominio sin dependencias de
/// frameworks (sin EF Core, sin ASP.NET). La representación persistente vive
/// aparte, en la capa de infraestructura, y se traduce mediante un mapper.</para>
///
/// <para><b>Inmutabilidad:</b> propiedades <c>init</c>-only (sin setters). El
/// estado se construye con inicializador de objeto y los cambios se modelan
/// reconstruyendo la entidad.</para>
/// </summary>
public abstract class Persona
{
    /// <summary>Identificador interno (clave primaria).</summary>
    public long? Id { get; init; }

    /// <summary>Nombre completo de la persona.</summary>
    public required string Nombre { get; init; }

    /// <summary>Género (value object de dominio).</summary>
    public Genero? Genero { get; init; }

    /// <summary>Edad en años.</summary>
    public int? Edad { get; init; }

    /// <summary>Documento de identidad. Único a nivel de negocio.</summary>
    public required string Identificacion { get; init; }

    /// <summary>Dirección de domicilio.</summary>
    public string? Direccion { get; init; }

    /// <summary>Número de teléfono de contacto.</summary>
    public string? Telefono { get; init; }
}
