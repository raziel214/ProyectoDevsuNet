namespace Devsu.Clientes.Domain.Exceptions;

/// <summary>
/// Se lanza al cambiar la contraseña cuando la contraseña actual provista no
/// coincide con la almacenada.
///
/// <para>Operación sensible de dominio bancario: el cambio de credencial exige
/// validar la contraseña vigente. El adaptador web la traduce a un HTTP 400.</para>
/// </summary>
public sealed class ContrasenaActualInvalidaException()
    : Exception("La contraseña actual es incorrecta.");
