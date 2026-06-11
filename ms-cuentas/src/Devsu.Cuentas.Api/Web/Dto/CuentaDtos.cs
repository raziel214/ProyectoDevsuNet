using System.ComponentModel.DataAnnotations;
using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Api.Web.Dto;

/// <summary>DTO de entrada para crear una cuenta (F1).</summary>
public record CuentaRequest
{
    [Required(ErrorMessage = "el numeroCuenta es obligatorio")]
    [StringLength(20)]
    public string NumeroCuenta { get; init; } = null!;

    [Required(ErrorMessage = "el tipoCuenta es obligatorio")]
    public TipoCuenta? TipoCuenta { get; init; }

    [Required(ErrorMessage = "el saldoInicial es obligatorio")]
    [Range(typeof(decimal), "0", "9999999999999.99", ErrorMessage = "el saldoInicial no puede ser negativo")]
    public decimal? SaldoInicial { get; init; }

    [Required(ErrorMessage = "el estado es obligatorio")]
    public bool? Estado { get; init; }

    [Required(ErrorMessage = "el clienteId es obligatorio")]
    [StringLength(50)]
    public string ClienteId { get; init; } = null!;
}

/// <summary>DTO de entrada para actualizar los datos editables de una cuenta (tipo).</summary>
public record ActualizarCuentaRequest
{
    [Required(ErrorMessage = "el tipoCuenta es obligatorio")]
    public TipoCuenta? TipoCuenta { get; init; }
}

/// <summary>DTO de entrada para habilitar/inhabilitar una cuenta (operación sensible).</summary>
public record CambiarEstadoCuentaRequest
{
    [Required(ErrorMessage = "el estado es obligatorio")]
    public bool? Estado { get; init; }
}

/// <summary>DTO de salida de una cuenta.</summary>
public record CuentaResponse(
    long? Id,
    string NumeroCuenta,
    TipoCuenta TipoCuenta,
    decimal SaldoInicial,
    decimal SaldoDisponible,
    bool Estado,
    string ClienteId
);
