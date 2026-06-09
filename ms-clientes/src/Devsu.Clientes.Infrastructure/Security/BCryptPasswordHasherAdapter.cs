using Devsu.Clientes.Application.Port.Out;

namespace Devsu.Clientes.Infrastructure.Security;

/// <summary>
/// Adaptador de salida que implementa <see cref="IPasswordHasherPort"/> con
/// <b>BCrypt</b> (algoritmo de hashing adaptativo con salt incorporado).
///
/// <para>La aplicación solo conoce el puerto; este detalle de seguridad vive en
/// infraestructura. BCrypt genera un salt aleatorio por hash, así que dos
/// contraseñas iguales producen hashes distintos.</para>
/// </summary>
public sealed class BCryptPasswordHasherAdapter : IPasswordHasherPort
{
    public string Hash(string rawPassword)
        => BCrypt.Net.BCrypt.HashPassword(rawPassword);
}
