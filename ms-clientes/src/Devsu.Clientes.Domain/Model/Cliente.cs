namespace Devsu.Clientes.Domain.Model;

/// <summary>
/// Agregado raíz del dominio: un Cliente.
///
/// <para>Hereda de <see cref="Persona"/> los datos personales (nombre, género,
/// edad, identificación, dirección, teléfono y el id/clave primaria) y añade los
/// atributos propios del cliente.</para>
///
/// <para>Es la <b>única entidad concreta</b> de este microservicio: sobre ella
/// operan los casos de uso (CRUD, F1). <see cref="Persona"/> es solo su base
/// abstracta.</para>
///
/// <para>Mantiene la pureza hexagonal (sin EF Core/ASP.NET) y la inmutabilidad
/// (propiedades <c>init</c>-only).</para>
/// </summary>
public sealed class Cliente : Persona
{
    /// <summary>Identificador de negocio / usuario de acceso. Clave única.</summary>
    public required string ClienteId { get; init; }

    /// <summary>Clave de acceso (hash). Dato sensible: nunca se expone en respuestas.</summary>
    public required string Contrasena { get; init; }

    /// <summary>Estado del cliente (activo/inactivo).</summary>
    public required bool Estado { get; init; }
}
