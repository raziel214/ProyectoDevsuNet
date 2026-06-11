using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Api.Web.Dto;

/// <summary>
/// DTO de salida del API para un cliente.
///
/// <para>Record inmutable. <b>No incluye la contraseña</b> (dato sensible: nunca
/// se expone en las respuestas).</para>
/// </summary>
public record ClienteResponse(
    long? Id,
    string Nombre,
    Genero? Genero,
    int? Edad,
    string Identificacion,
    string? Direccion,
    string? Telefono,
    string ClienteId,
    bool Estado
);
