using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Application.Port.In;

/// <summary>
/// Datos de entrada para actualizar un cliente (caso de uso
/// <see cref="IActualizarClienteUseCase"/>).
///
/// <para>Incluye los campos modificables. No incluye <c>Identificacion</c> ni
/// <c>ClienteId</c> porque son claves de identidad del cliente (no se cambian por
/// una actualización); el cliente a modificar se ubica por su <c>Id</c>.</para>
///
/// <para>La <c>Contrasena</c> es opcional: si llega, se re-hashea; si es nula, se
/// conserva la actual.</para>
/// </summary>
public record ActualizarClienteCommand(
    string Nombre,
    Genero? Genero,
    int? Edad,
    string? Direccion,
    string? Telefono,
    string? Contrasena,
    bool Estado
);
