using System.ComponentModel.DataAnnotations;
using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Api.Web.Dto;

/// <summary>Valida que un valor numérico sea distinto de cero.</summary>
public sealed class DistintoDeCeroAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
        => value is decimal d && d == 0m
            ? new ValidationResult(ErrorMessage ?? "el valor no puede ser cero")
            : ValidationResult.Success;
}

/// <summary>DTO de entrada para registrar un movimiento (F2/F3). Valor con signo, ≠ 0.</summary>
public record MovimientoRequest
{
    [Required(ErrorMessage = "el numeroCuenta es obligatorio")]
    [StringLength(20)]
    public string NumeroCuenta { get; init; } = null!;

    [Required(ErrorMessage = "el valor es obligatorio")]
    [DistintoDeCero(ErrorMessage = "el valor no puede ser cero")]
    public decimal? Valor { get; init; }
}

/// <summary>DTO de entrada para editar la descripción (única parte mutable del ledger).</summary>
public record ActualizarMovimientoRequest
{
    [StringLength(255)]
    public string? Descripcion { get; init; }
}

/// <summary>DTO de salida de un movimiento.</summary>
public record MovimientoResponse(
    long? Id,
    DateTime Fecha,
    TipoMovimiento TipoMovimiento,
    decimal Valor,
    decimal Saldo,
    string NumeroCuenta,
    string? Descripcion
);
