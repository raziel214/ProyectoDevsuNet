using System.ComponentModel.DataAnnotations;
using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Api.Web.Dto;

/// <summary>
/// DTO de entrada para actualizar el <b>perfil</b> de un cliente (datos
/// personales). No incluye identidad, estado ni contraseña: esas son operaciones
/// separadas (banca estricta).
/// </summary>
public record ActualizarPerfilRequest
{
    [Required(ErrorMessage = "el nombre es obligatorio")]
    [StringLength(120)]
    public string Nombre { get; init; } = null!;

    public Genero? Genero { get; init; }

    [Range(0, 120, ErrorMessage = "la edad debe estar entre 0 y 120")]
    public int? Edad { get; init; }

    [StringLength(200)]
    public string? Direccion { get; init; }

    [StringLength(20)]
    public string? Telefono { get; init; }
}
