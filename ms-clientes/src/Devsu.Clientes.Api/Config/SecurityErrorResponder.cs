using Devsu.Clientes.Api.Web.Dto;

namespace Devsu.Clientes.Api.Config;

/// <summary>
/// Escribe los errores de seguridad (401 / 403) con el MISMO formato
/// <see cref="ErrorResponse"/> que el resto del API.
///
/// <para>Necesario porque estos errores los produce el middleware de
/// autenticación/autorización <b>antes</b> de llegar al controller, por lo que el
/// <c>GlobalExceptionHandler</c> no los captura. Se engancha en los eventos de
/// <c>JwtBearer</c>.</para>
/// </summary>
public static class SecurityErrorResponder
{
    public static Task WriteUnauthorizedAsync(HttpContext context)
        => WriteAsync(context, StatusCodes.Status401Unauthorized,
            "Unauthorized", "Token ausente, inválido o expirado");

    public static Task WriteForbiddenAsync(HttpContext context)
        => WriteAsync(context, StatusCodes.Status403Forbidden,
            "Forbidden", "No tiene permisos para acceder a este recurso");

    private static async Task WriteAsync(HttpContext context, int status, string error, string message)
    {
        // Evita "headers already sent" si la respuesta ya comenzó.
        if (context.Response.HasStarted) return;

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        var body = new ErrorResponse(
            Timestamp: DateTimeOffset.UtcNow,
            Status: status,
            Error: error,
            Message: message,
            Path: context.Request.Path);

        await context.Response.WriteAsJsonAsync(body);
    }
}
