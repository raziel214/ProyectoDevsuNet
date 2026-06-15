using Devsu.Clientes.Application.Port.In;
using Devsu.Clientes.Application.Port.Out;
using Devsu.Clientes.Domain.Exceptions;
using Devsu.Clientes.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Devsu.Clientes.Application.Service;

/// <summary>
/// Servicio de aplicación: implementa los casos de uso del CRUD de Cliente (F1) y
/// publica eventos de dominio para sincronizar el microservicio de Cuentas
/// (comunicación asíncrona).
///
/// <para>Las operaciones sensibles (cambio de estado y de contraseña) están
/// separadas del perfil y se registran en el log de auditoría.</para>
/// </summary>
public sealed class ClienteService(
    IClienteRepositoryPort clienteRepository,
    IPasswordHasherPort passwordHasher,
    IClienteEventPublisherPort eventPublisher,
    ILogger<ClienteService> logger)
    : ICrearClienteUseCase,
      IConsultarClienteUseCase,
      IActualizarClienteUseCase,
      ICambiarEstadoClienteUseCase,
      ICambiarContrasenaClienteUseCase,
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

    /// <summary>Actualiza solo los datos de perfil; conserva identidad, estado y contraseña.</summary>
    public async Task<Cliente> ActualizarAsync(long id, ActualizarPerfilCommand command, CancellationToken ct = default)
    {
        var existente = await clienteRepository.BuscarPorIdAsync(id, ct)
                        ?? throw new ClienteNoEncontradoException(id);

        var actualizado = new Cliente
        {
            Id = existente.Id,
            Identificacion = existente.Identificacion, // identidad: no cambia
            ClienteId = existente.ClienteId,           // identidad: no cambia
            Contrasena = existente.Contrasena,         // credencial: operación aparte
            Estado = existente.Estado,                 // estado: operación aparte
            Nombre = command.Nombre,
            Genero = command.Genero,
            Edad = command.Edad,
            Direccion = command.Direccion,
            Telefono = command.Telefono
        };

        var guardado = await clienteRepository.GuardarAsync(actualizado, ct);
        await eventPublisher.PublicarActualizadoAsync(guardado, ct);
        return guardado;
    }

    /// <summary>Operación sensible: habilita/inhabilita un cliente (auditada).</summary>
    public async Task<Cliente> CambiarEstadoAsync(long id, bool estado, CancellationToken ct = default)
    {
        var existente = await clienteRepository.BuscarPorIdAsync(id, ct)
                        ?? throw new ClienteNoEncontradoException(id);

        var actualizado = Reconstruir(existente, estado: estado);
        var guardado = await clienteRepository.GuardarAsync(actualizado, ct);

        logger.LogInformation("AUDIT cambio de estado clienteId={ClienteId} estado={Estado}",
            guardado.ClienteId, estado);

        await eventPublisher.PublicarActualizadoAsync(guardado, ct);
        return guardado;
    }

    /// <summary>Operación sensible: cambia la contraseña verificando la actual (auditada).</summary>
    public async Task CambiarContrasenaAsync(long id, CambiarContrasenaCommand command, CancellationToken ct = default)
    {
        var existente = await clienteRepository.BuscarPorIdAsync(id, ct)
                        ?? throw new ClienteNoEncontradoException(id);

        if (!passwordHasher.Verify(command.ContrasenaActual, existente.Contrasena))
            throw new ContrasenaActualInvalidaException();

        var actualizado = Reconstruir(existente, contrasena: passwordHasher.Hash(command.ContrasenaNueva));
        await clienteRepository.GuardarAsync(actualizado, ct);

        // No se publica evento: la réplica de cuentas no usa la contraseña.
        logger.LogInformation("AUDIT cambio de contrasena clienteId={ClienteId}", existente.ClienteId);
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

    /// <summary>Reconstruye el agregado conservando su identidad y cambiando solo lo indicado.</summary>
    private static Cliente Reconstruir(Cliente origen, bool? estado = null, string? contrasena = null) => new()
    {
        Id = origen.Id,
        Nombre = origen.Nombre,
        Genero = origen.Genero,
        Edad = origen.Edad,
        Identificacion = origen.Identificacion,
        Direccion = origen.Direccion,
        Telefono = origen.Telefono,
        ClienteId = origen.ClienteId,
        Contrasena = contrasena ?? origen.Contrasena,
        Estado = estado ?? origen.Estado
    };
}
