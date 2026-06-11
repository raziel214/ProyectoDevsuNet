namespace Devsu.Clientes.Api.Web.Dto;

/// <summary>
/// Cuerpo estándar de error del API (RFC-7807 simplificado).
/// Lo construye el manejador global de excepciones.
/// </summary>
public record ErrorResponse(
    DateTimeOffset Timestamp,
    int Status,
    string Error,
    string Message,
    string Path
);
