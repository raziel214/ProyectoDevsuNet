using System.ComponentModel.DataAnnotations;

namespace Devsu.Clientes.Api.Web.Dto;

/// <summary>
/// DTO de entrada para cambiar la contraseña de un cliente. Exige la contraseña
/// actual (se verifica) además de la nueva.
/// </summary>
public record CambiarContrasenaRequest
{
    [Required(ErrorMessage = "la contraseña actual es obligatoria")]
    [StringLength(100, MinimumLength = 4)]
    public string ContrasenaActual { get; init; } = null!;

    [Required(ErrorMessage = "la contraseña nueva es obligatoria")]
    [StringLength(100, MinimumLength = 4)]
    public string ContrasenaNueva { get; init; } = null!;
}
