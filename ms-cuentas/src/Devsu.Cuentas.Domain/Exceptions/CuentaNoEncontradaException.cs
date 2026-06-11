namespace Devsu.Cuentas.Domain.Exceptions;

/// <summary>
/// Se lanza cuando se busca una cuenta que no existe. El adaptador web la
/// traduce a un HTTP 404.
/// </summary>
public sealed class CuentaNoEncontradaException : Exception
{
    public CuentaNoEncontradaException(long id)
        : base($"Cuenta no encontrada con id: {id}") { }

    private CuentaNoEncontradaException(string message) : base(message) { }

    public static CuentaNoEncontradaException PorNumero(string numeroCuenta)
        => new($"Cuenta no encontrada: {numeroCuenta}");
}
