using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Application.Port.In;

/// <summary>
/// Datos de entrada para actualizar el <b>perfil</b> de un cliente (caso de uso
/// <see cref="IActualizarClienteUseCase"/>).
///
/// <para>Solo incluye los datos personales modificables. <b>No</b> incluye la
/// identidad (<c>identificacion</c>, <c>clienteId</c>), ni el <c>estado</c>, ni la
/// <c>contrasena</c>: en el dominio bancario esas son operaciones sensibles y
/// separadas (ver <see cref="ICambiarEstadoClienteUseCase"/> y
/// <see cref="ICambiarContrasenaClienteUseCase"/>).</para>
/// </summary>
public record ActualizarPerfilCommand(
    string Nombre,
    Genero? Genero,
    int? Edad,
    string? Direccion,
    string? Telefono
);
