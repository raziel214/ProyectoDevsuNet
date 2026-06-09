namespace Devsu.Clientes.Infrastructure.Vault;

/// <summary>
/// Configuración del acceso a HashiCorp Vault (sección <c>Vault</c>).
///
/// <para>El microservicio lee de Vault las credenciales de BD y RabbitMQ que
/// sembró <c>infra/vault/seed-secrets.sh</c> en el path <c>secret/ms-clientes</c>
/// (KV v2). Las credenciales nunca se hardcodean ni se commitean.</para>
/// </summary>
public sealed class VaultOptions
{
    public const string SectionName = "Vault";

    /// <summary>Si está deshabilitado, se usan las credenciales de configuración
    /// (útil para pruebas de integración sin Vault).</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Dirección del servidor Vault (env VAULT_ADDR / VAULT_URI).</summary>
    public string Address { get; set; } = "http://localhost:8200";

    /// <summary>Punto de montaje del motor KV v2.</summary>
    public string MountPoint { get; set; } = "secret";

    /// <summary>Ruta del secreto dentro del motor KV.</summary>
    public string SecretPath { get; set; } = "ms-clientes";

    /// <summary>Token directo (si se provee, omite el login userpass).
    /// Equivale a SPRING_CLOUD_VAULT_TOKEN / VAULT_TOKEN.</summary>
    public string? Token { get; set; }

    /// <summary>Usuario para autenticación userpass.</summary>
    public string Username { get; set; } = "devsu-reader";

    /// <summary>Contraseña userpass (env VAULT_USERPASS_PASSWORD; nunca se commitea).</summary>
    public string? Password { get; set; }
}
