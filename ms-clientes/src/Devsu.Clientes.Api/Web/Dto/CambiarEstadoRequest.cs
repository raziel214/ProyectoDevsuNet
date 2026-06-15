using System.ComponentModel.DataAnnotations;

namespace Devsu.Clientes.Api.Web.Dto;

/// <summary>
/// DTO de entrada para habilitar/inhabilitar un cliente (operación sensible).
/// </summary>
public record CambiarEstadoRequest
{
    [Required(ErrorMessage = "el estado es obligatorio")]
    public bool? Estado { get; init; }
}
