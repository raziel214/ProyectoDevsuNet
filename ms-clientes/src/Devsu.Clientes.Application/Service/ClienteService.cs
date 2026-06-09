using Devsu.Clientes.Application.Port.In;
using Devsu.Clientes.Application.Port.Out;
using Devsu.Clientes.Domain.Exceptions;
using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Application.Service;

/// <summary>
/// Servicio de aplicación: implementa los casos de uso del CRUD de Cliente (F1) y
/// publica eventos de dominio para sincronizar el microservicio de Cuentas
/// (comunicación asíncrona).
/// </summary>
public sealed class ClienteService(
    IClienteRepositoryPort clienteRepository,
    IPasswordHasherPort passwordHasher,
    IClienteEventPublisherPort eventPublisher)
    : ICrearClienteUseCase,
      IConsultarClienteUseCase,
      IActualizarClienteUseCase,
      IEliminarClienteUseCase
{
    public async Task<Cliente> CrearAsync(CrearClienteCommand command, CancellationToken ct = default)
    {
        await ValidarUnicidadAsync(command.ClienteId, command.Identificacion, ct);

        var cliente = new Cliente
        {
            Nombre = command.Nombre,
            Genero = command.Genero,
            Edad = command.Edad,
            Identificacion = command.Identificacion,
            Direccion = command.Direccion,
            Telefono = command.Telefono,
            ClienteId = command.ClienteId,
            Contrasena = passwordHasher.Hash(command.Contrasena),
            Estado = command.Estado ?? true
        };

        var guardado = await clienteRepository.GuardarAsync(cliente, ct);
        await eventPublisher.PublicarCreadoAsync(guardado, ct);
        return guardado;
    }

    public async Task<Cliente> BuscarPorIdAsync(long id, CancellationToken ct = default)
        => await clienteRepository.BuscarPorIdAsync(id, ct)
           ?? throw new ClienteNoEncontradoException(id);

    public Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken ct = default)
        => clienteRepository.ListarAsync(ct);

    public async Task<Cliente> ActualizarAsync(long id, ActualizarClienteCommand command, CancellationToken ct = default)
    {
        var existente = await clienteRepository.BuscarPorIdAsync(id, ct)
                        ?? throw new ClienteNoEncontradoException(id);

        // La contraseña solo se re-hashea si llega una nueva; si no, se conserva.
        var contrasena = command.Contrasena is not null
            ? passwordHasher.Hash(command.Contrasena)
            : existente.Contrasena;

        // Inmutabilidad: se reconstruye el agregado conservando su identidad.
        var actualizado = new Cliente
        {
            Id = existente.Id,
            Identificacion = existente.Identificacion, // identidad: no cambia
            ClienteId = existente.ClienteId,           // identidad: no cambia
            Nombre = command.Nombre,
            Genero = command.Genero,
            Edad = command.Edad,
            Direccion = command.Direccion,
            Telefono = command.Telefono,
            Contrasena = contrasena,
            Estado = command.Estado
        };

        var guardado = await clienteRepository.GuardarAsync(actualizado, ct);
        await eventPublisher.PublicarActualizadoAsync(guardado, ct);
        return guardado;
    }

    public async Task EliminarAsync(long id, CancellationToken ct = default)
    {
        var existente = await clienteRepository.BuscarPorIdAsync(id, ct)
                        ?? throw new ClienteNoEncontradoException(id);
        await clienteRepository.EliminarPorIdAsync(id, ct);
        await eventPublisher.PublicarEliminadoAsync(existente.ClienteId, ct);
    }

    /// <summary>Garantiza que clienteId e identificación no estén ya registrados.</summary>
    private async Task ValidarUnicidadAsync(string clienteId, string identificacion, CancellationToken ct)
    {
        if (await clienteRepository.ExistePorClienteIdAsync(clienteId, ct))
            throw new ClienteDuplicadoException("clienteId", clienteId);

        if (await clienteRepository.ExistePorIdentificacionAsync(identificacion, ct))
            throw new ClienteDuplicadoException("identificacion", identificacion);
    }
}
