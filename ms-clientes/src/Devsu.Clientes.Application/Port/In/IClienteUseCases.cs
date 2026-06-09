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

/// <summary>Caso de uso: actualizar un cliente (F1).</summary>
public interface IActualizarClienteUseCase
{
    Task<Cliente> ActualizarAsync(long id, ActualizarClienteCommand command, CancellationToken ct = default);
}

/// <summary>Caso de uso: eliminar un cliente (F1).</summary>
public interface IEliminarClienteUseCase
{
    Task EliminarAsync(long id, CancellationToken ct = default);
}
