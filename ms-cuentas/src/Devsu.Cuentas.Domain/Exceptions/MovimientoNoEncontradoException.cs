namespace Devsu.Cuentas.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se busca un movimiento que no existe. El adaptador web la
/// traduce a un HTTP 404.
/// </summary>
public sealed class MovimientoNoEncontradoException(long id)
    : Exception($"Movimiento no encontrado con id: {id}");
