namespace Devsu.Cuentas.Domain.Exceptions;

/// <summary>
/// Se lanza al crear una cuenta cuando el cliente referenciado no existe en la
/// réplica local (<c>cliente_ref</c>). El adaptador web la traduce a un HTTP 404.
/// </summary>
public sealed class ClienteNoEncontradoException(string clienteId)
    : Exception($"El cliente no existe: {clienteId}");
