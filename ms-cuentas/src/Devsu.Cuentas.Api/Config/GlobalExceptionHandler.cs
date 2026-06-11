using Devsu.Cuentas.Api.Web.Dto;
using Devsu.Cuentas.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Devsu.Cuentas.Api.Config;

/// <summary>
/// Manejo centralizado de excepciones. Traduce las excepciones de dominio a
/// respuestas HTTP con el cuerpo <see cref="ErrorResponse"/>. Incluye el 422
/// "Saldo no disponible" (F3).
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception switch
        {
            CuentaNoEncontradaException => StatusCodes.Status404NotFound,
            MovimientoNoEncontradoException => StatusCodes.Status404NotFound,
            ClienteNoEncontradoException => StatusCodes.Status404NotFound,
            NumeroCuentaDuplicadoException => StatusCodes.Status409Conflict,
            SaldoNoDisponibleException => StatusCodes.Status422UnprocessableEntity,
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
        StatusCodes.Status422UnprocessableEntity => "Unprocessable Entity",
        _ => "Internal Server Error"
    };
}
