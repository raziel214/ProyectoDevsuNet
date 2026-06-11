namespace Devsu.Cuentas.Infrastructure.Vault;

/// <summary>
/// Configuración del acceso a HashiCorp Vault (sección <c>Vault</c>). El
/// microservicio lee de Vault las credenciales de BD y RabbitMQ sembradas en
/// <c>secret/dev/devsu/ms-cuentas</c>.
/// </summary>
public sealed class VaultOptions
{
    public const string SectionName = "Vault";

    public bool Enabled { get; set; } = true;
    public string Address { get; set; } = "http://localhost:8200";
    public string MountPoint { get; set; } = "secret";
    public string SecretPath { get; set; } = "ms-cuentas";

    /// <summary>Token directo (si se provee, omite el login userpass).</summary>
    public string? Token { get; set; }

    public string Username { get; set; } = "devsu-reader";

    /// <summary>Contraseña userpass (env VAULT_USERPASS_PASSWORD; nunca se commitea).</summary>
    public string? Password { get; set; }
}
