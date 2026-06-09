using Devsu.Clientes.Api.Web.Dto;
using Devsu.Clientes.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Devsu.Clientes.Api.Config;

/// <summary>
/// Manejo centralizado de excepciones. Traduce las excepciones de dominio a
/// respuestas HTTP consistentes con el cuerpo <see cref="ErrorResponse"/>.
///
/// <para>Es el equivalente .NET de un <c>@RestControllerAdvice</c>: se registra
/// con <c>AddExceptionHandler</c> y captura cualquier excepción no manejada.</para>
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            ClienteNoEncontradoException => StatusCodes.Status404NotFound,
            ClienteDuplicadoException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Error no controlado: {Message}", exception.Message);
        else
            logger.LogWarning("{Status}: {Message}", status, exception.Message);

        var body = new ErrorResponse(
            Timestamp: DateTimeOffset.UtcNow,
            Status: status,
            Error: ReasonPhrase(status),
            Message: exception.Message,
            Path: httpContext.Request.Path);

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(body, cancellationToken);
        return true;
    }

    private static string ReasonPhrase(int status) => status switch
    {
        StatusCodes.Status400BadRequest => "Bad Request",
        StatusCodes.Status404NotFound => "Not Found",
        StatusCodes.Status409Conflict => "Conflict",
        _ => "Internal Server Error"
    };
}
