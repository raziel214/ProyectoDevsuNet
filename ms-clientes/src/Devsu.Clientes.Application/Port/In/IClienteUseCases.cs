using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Application.Port.In;

/// <summary>Caso de uso: crear un cliente (F1).</summary>
public interface ICrearClienteUseCase
{
    Task<Cliente> CrearAsync(CrearClienteCommand command, CancellationToken ct = default);
}

/// <summary>Caso de uso: consultar clientes (F1).</summary>
public interface IConsultarClienteUseCase
{
    Task<Cliente> BuscarPorIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken ct = default);
}

/// <summary>Caso de uso: actualizar el perfil de un cliente (datos personales).</summary>
public interface IActualizarClienteUseCase
{
    Task<Cliente> ActualizarAsync(long id, ActualizarPerfilCommand command, CancellationToken ct = default);
}

/// <summary>
/// Caso de uso sensible: habilitar/inhabilitar un cliente. Separado del perfil
/// por ser una operación auditable de dominio bancario.
/// </summary>
public interface ICambiarEstadoClienteUseCase
{
    Task<Cliente> CambiarEstadoAsync(long id, bool estado, CancellationToken ct = default);
}

/// <summary>
/// Caso de uso sensible: cambiar la contraseña (credencial) de un cliente,
/// verificando la contraseña actual. Separado del perfil y auditable.
/// </summary>
public interface ICambiarContrasenaClienteUseCase
{
    Task CambiarContrasenaAsync(long id, CambiarContrasenaCommand command, CancellationToken ct = default);
}

/// <summary>Caso de uso: eliminar un cliente (F1).</summary>
public interface IEliminarClienteUseCase
{
    Task EliminarAsync(long id, CancellationToken ct = default);
}
