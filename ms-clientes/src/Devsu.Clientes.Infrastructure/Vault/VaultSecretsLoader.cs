using Microsoft.Extensions.Logging;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;

namespace Devsu.Clientes.Infrastructure.Vault;

/// <summary>
/// Carga los secretos (credenciales de BD y RabbitMQ) desde Vault al arrancar,
/// replicando el flujo del proyecto Java: autenticación <b>userpass</b> (o token
/// directo) y lectura del secreto KV v2 <c>secret/ms-clientes</c>.
///
/// <para>Fail-fast: si Vault está habilitado y no se puede resolver el secreto,
/// la aplicación no arranca (las credenciales son obligatorias).</para>
/// </summary>
public static class VaultSecretsLoader
{
    public static async Task<VaultSecrets> LoadAsync(
        VaultOptions options, ILogger logger, CancellationToken ct = default)
    {
        IAuthMethodInfo authMethod;
        if (!string.IsNullOrEmpty(options.Token))
        {
            logger.LogInformation("[Vault] Usando token directo; se omite el login userpass.");
            authMethod = new TokenAuthMethodInfo(options.Token);
        }
        else
        {
            if (string.IsNullOrEmpty(options.Password))
                throw new InvalidOperationException(
                    "Falta la contraseña de Vault. Define VAULT_USERPASS_PASSWORD (env) o Vault:Password.");

            logger.LogInformation("[Vault] Autenticando en {Address} como {User} (userpass)...",
                options.Address, options.Username);
            authMethod = new UserPassAuthMethodInfo(options.Username, options.Password);
        }

        var client = new VaultClient(new VaultClientSettings(options.Address, authMethod));

        try
        {
            var kv = await client.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                path: options.SecretPath, mountPoint: options.MountPoint);

            var data = kv.Data.Data;
            var secrets = new VaultSecrets(
                DbUsername: Require(data, VaultSecrets.KeyDbUsername),
                DbPassword: Require(data, VaultSecrets.KeyDbPassword),
                RabbitUsername: Require(data, VaultSecrets.KeyRabbitUsername),
                RabbitPassword: Require(data, VaultSecrets.KeyRabbitPassword));

            logger.LogInformation("[Vault] Secretos de {Path} cargados correctamente.", options.SecretPath);
            return secrets;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Vault] No se pudieron resolver los secretos de {Path}.", options.SecretPath);
            throw new InvalidOperationException("No se puede arrancar sin los secretos de Vault.", ex);
        }
    }

    private static string Require(IDictionary<string, object> data, string key)
        => data.TryGetValue(key, out var value) && value is not null
            ? value.ToString()!
            : throw new InvalidOperationException($"Falta la clave '{key}' en el secreto de Vault.");
}
