using Devsu.Clientes.Application.Port.In;
using Devsu.Clientes.Api.Web.Dto;
using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Api.Web.Mapper;

/// <summary>
/// Traduce entre los DTOs del API y los commands/dominio de la aplicación.
/// Mantiene el caso de uso aislado de la representación HTTP/JSON.
/// </summary>
public static class ClienteWebMapper
{
    public static CrearClienteCommand ToCrearCommand(ClienteRequest r) => new(
        r.Nombre, r.Genero, r.Edad, r.Identificacion,
        r.Direccion, r.Telefono, r.ClienteId, r.Contrasena, r.Estado);

    /// <summary>En actualización no se cambian identificación ni clienteId (identidad).</summary>
    public static ActualizarClienteCommand ToActualizarCommand(ClienteRequest r) => new(
        r.Nombre, r.Genero, r.Edad,
        r.Direccion, r.Telefono, r.Contrasena, r.Estado ?? true);

    public static ClienteResponse ToResponse(Cliente c) => new(
        c.Id, c.Nombre, c.Genero, c.Edad,
        c.Identificacion, c.Direccion, c.Telefono,
        c.ClienteId, c.Estado);
}
