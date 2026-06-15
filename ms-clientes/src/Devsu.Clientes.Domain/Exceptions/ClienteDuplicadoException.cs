namespace Devsu.Clientes.Domain.Exceptions;

/// <summary>
/// Se lanza al intentar crear un cliente con un valor único ya existente
/// (clienteId o identificación).
///
/// <para>Excepción de dominio. El adaptador web la traduce a un HTTP 409 Conflict.</para>
/// </summary>
public sealed class ClienteDuplicadoException(string campo, string valor)
    : Exception($"Ya existe un cliente con {campo}: {valor}");
