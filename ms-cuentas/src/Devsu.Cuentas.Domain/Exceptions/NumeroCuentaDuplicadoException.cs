namespace Devsu.Cuentas.Domain.Exceptions;

/// <summary>
/// Se lanza al crear una cuenta con un número ya existente. El adaptador web la
/// traduce a un HTTP 409.
/// </summary>
public sealed class NumeroCuentaDuplicadoException(string numeroCuenta)
    : Exception($"Ya existe una cuenta con número: {numeroCuenta}");
