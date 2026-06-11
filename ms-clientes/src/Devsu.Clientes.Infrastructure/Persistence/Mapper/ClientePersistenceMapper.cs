using Devsu.Clientes.Domain.Model;
using Devsu.Clientes.Infrastructure.Persistence.Entity;

namespace Devsu.Clientes.Infrastructure.Persistence.Mapper;

/// <summary>
/// Traduce entre el agregado de dominio <see cref="Cliente"/> (inmutable, puro) y
/// la entidad de persistencia <see cref="ClienteEntity"/> (EF Core, mutable).
///
/// <para>Esta traducción es lo que permite mantener el dominio libre de EF Core
/// en el Hexagonal estricto. Es un mapper manual (explícito y sin dependencias
/// extra).</para>
/// </summary>
public static class ClientePersistenceMapper
{
    /// <summary>Dominio → entidad EF (para guardar).</summary>
    public static ClienteEntity ToEntity(Cliente cliente) => new()
    {
        Id = cliente.Id ?? 0,
        Nombre = cliente.Nombre,
        Genero = cliente.Genero,
        Edad = cliente.Edad,
        Identificacion = cliente.Identificacion,
        Direccion = cliente.Direccion,
        Telefono = cliente.Telefono,
        ClienteId = cliente.ClienteId,
        Contrasena = cliente.Contrasena,
        Estado = cliente.Estado
    };

    /// <summary>Entidad EF → dominio (al leer).</summary>
    public static Cliente ToDomain(ClienteEntity entity) => new()
    {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Genero = entity.Genero,
        Edad = entity.Edad,
        Identificacion = entity.Identificacion,
        Direccion = entity.Direccion,
        Telefono = entity.Telefono,
        ClienteId = entity.ClienteId,
        Contrasena = entity.Contrasena,
        Estado = entity.Estado
    };
}
