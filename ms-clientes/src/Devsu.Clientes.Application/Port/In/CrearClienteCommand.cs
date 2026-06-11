using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Application.Port.In;

/// <summary>
/// Datos de entrada para crear un cliente (caso de uso <see cref="ICrearClienteUseCase"/>).
///
/// <para>Es un <c>record</c> (inmutable, conciso) que desacopla el caso de uso del
/// mundo externo: el controller traduce el DTO web a este command, de modo que la
/// aplicación no conoce la representación HTTP/JSON.</para>
///
/// <para>No incluye <c>Id</c> porque al crear aún no existe (lo asigna la BD).
/// La <c>Contrasena</c> llega en texto plano y se hashea antes de persistir.</para>
/// </summary>
public record CrearClienteCommand(
    string Nombre,
    Genero? Genero,
    int? Edad,
    string Identificacion,
    string? Direccion,
    string? Telefono,
    string ClienteId,
    string Contrasena,
    bool? Estado
);
