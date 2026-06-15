namespace Devsu.Clientes.Infrastructure.Vault;

/// <summary>
/// Configuración del acceso a HashiCorp Vault (sección <c>Vault</c>).
///
/// <para>Los valores provienen de la configuración (appsettings.json) y se pueden
/// sobrescribir por variables de entorno en el deploy (imagen inmutable + config
/// externa). Las credenciales nunca se hardcodean ni se commitean.</para>
/// </summary>
public sealed class VaultOptions
{
    public const string SectionName = "Vault";

    /// <summary>Default seguro (fail-closed): si no se configura, se intenta Vault.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Dirección del servidor Vault (env VAULT_ADDR / VAULT_URI).</summary>
    public string Address { get; set; } = default!;

    /// <summary>Punto de montaje del motor KV v2.</summary>
    public string MountPoint { get; set; } = default!;

    /// <summary>Ruta del secreto dentro del motor KV.</summary>
    public string SecretPath { get; set; } = default!;

    /// <summary>Token directo (si se provee, omite el login userpass).
    /// Equivale a SPRING_CLOUD_VAULT_TOKEN / VAULT_TOKEN.</summary>
    public string? Token { get; set; }

    /// <summary>Usuario para autenticación userpass.</summary>
    public string Username { get; set; } = default!;

    /// <summary>Contraseña userpass (env VAULT_USERPASS_PASSWORD; nunca se commitea).</summary>
    public string? Password { get; set; }
}
