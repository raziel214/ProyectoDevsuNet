using System.ComponentModel.DataAnnotations;
using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Api.Web.Dto;

/// <summary>
/// DTO de entrada del API para crear/actualizar un cliente.
///
/// <para>Record inmutable con validaciones (DataAnnotations). El controller lo
/// traduce a un command de aplicación; así el caso de uso no conoce la web.</para>
/// </summary>
public record ClienteRequest
{
    [Required(ErrorMessage = "el nombre es obligatorio")]
    [StringLength(120)]
    public string Nombre { get; init; } = null!;

    public Genero? Genero { get; init; }

    [Range(0, 120, ErrorMessage = "la edad debe estar entre 0 y 120")]
    public int? Edad { get; init; }

    [Required(ErrorMessage = "la identificación es obligatoria")]
    [StringLength(20)]
    public string Identificacion { get; init; } = null!;

    [StringLength(200)]
    public string? Direccion { get; init; }

    [StringLength(20)]
    public string? Telefono { get; init; }

    [Required(ErrorMessage = "el clienteId es obligatorio")]
    [StringLength(50)]
    public string ClienteId { get; init; } = null!;

    [Required(ErrorMessage = "la contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 4)]
    public string Contrasena { get; init; } = null!;

    [Required(ErrorMessage = "el estado es obligatorio")]
    public bool? Estado { get; init; }
}
