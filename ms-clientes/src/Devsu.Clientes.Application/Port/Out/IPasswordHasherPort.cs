namespace Devsu.Clientes.Application.Port.Out;

/// <summary>
/// Puerto de salida para el hasheo de contraseñas.
///
/// <para>La aplicación necesita guardar la contraseña <b>hasheada</b> (nunca en
/// texto plano), pero no debe depender de una librería concreta de seguridad.
/// Define este contrato; el adaptador en <c>Infrastructure.Security</c> lo
/// implementa con <b>BCrypt</b>.</para>
/// </summary>
public interface IPasswordHasherPort
{
    /// <summary>Devuelve el hash seguro (BCrypt) de una contraseña en texto plano.</summary>
    string Hash(string rawPassword);

    /// <summary>Verifica que una contraseña en texto plano corresponda a un hash.</summary>
    bool Verify(string rawPassword, string hash);
}
