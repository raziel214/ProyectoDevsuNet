using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Application.Port.Out;

/// <summary>
/// Puerto de salida para la persistencia de <see cref="Cliente"/>.
///
/// <para><b>Inversión de dependencias (SOLID-D):</b> esta interfaz pertenece a la
/// capa de aplicación y expresa <i>qué</i> necesita el dominio de un repositorio,
/// en términos del propio dominio (entidad <see cref="Cliente"/>). La
/// implementación concreta (EF Core) vive en <c>Infrastructure.Persistence</c>
/// como un adaptador.</para>
///
/// <para>Así, la aplicación no conoce EF Core ni la base de datos: solo conoce
/// este contrato. La flecha de dependencia apunta hacia adentro.</para>
/// </summary>
public interface IClienteRepositoryPort
{
    /// <summary>Crea o actualiza un cliente y devuelve el estado persistido (con id).</summary>
    Task<Cliente> GuardarAsync(Cliente cliente, CancellationToken ct = default);

    /// <summary>Busca un cliente por su clave primaria interna (null si no existe).</summary>
    Task<Cliente?> BuscarPorIdAsync(long id, CancellationToken ct = default);

    /// <summary>Busca un cliente por su identificador de negocio (clave única).</summary>
    Task<Cliente?> BuscarPorClienteIdAsync(string clienteId, CancellationToken ct = default);

    /// <summary>Lista todos los clientes.</summary>
    Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken ct = default);

    /// <summary>Elimina un cliente por su clave primaria interna.</summary>
    Task EliminarPorIdAsync(long id, CancellationToken ct = default);

    /// <summary>Indica si ya existe un cliente con ese clienteId (validación de unicidad).</summary>
    Task<bool> ExistePorClienteIdAsync(string clienteId, CancellationToken ct = default);

    /// <summary>Indica si ya existe un cliente con esa identificación (validación de unicidad).</summary>
    Task<bool> ExistePorIdentificacionAsync(string identificacion, CancellationToken ct = default);
}
