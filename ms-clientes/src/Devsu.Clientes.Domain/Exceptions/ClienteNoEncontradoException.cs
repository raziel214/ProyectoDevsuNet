namespace Devsu.Clientes.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se busca un cliente que no existe.
///
/// <para>Excepción de dominio (sin dependencias de frameworks). El adaptador web
/// la traduce a un HTTP 404 Not Found.</para>
/// </summary>
public sealed class ClienteNoEncontradoException(long id)
    : Exception($"Cliente no encontrado con id: {id}");
