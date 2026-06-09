namespace Devsu.Clientes.Domain.Model;

/// <summary>
/// Género de una <see cref="Persona"/>.
///
/// <para>Value object del dominio: un conjunto cerrado de valores válidos.
/// Usar un enum en lugar de un <c>string</c> evita valores mágicos e inválidos
/// y aporta tipado fuerte.</para>
/// </summary>
public enum Genero
{
    MASCULINO,
    FEMENINO,
    OTRO
}
