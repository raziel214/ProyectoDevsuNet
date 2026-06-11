namespace Devsu.Cuentas.Infrastructure.Vault;

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

    public string Address { get; set; } = default!;
    public string MountPoint { get; set; } = default!;
    public string SecretPath { get; set; } = default!;

    /// <summary>Token directo (si se provee, omite el login userpass).</summary>
    public string? Token { get; set; }

    public string Username { get; set; } = default!;

    /// <summary>Contraseña userpass (env VAULT_USERPASS_PASSWORD; nunca se commitea).</summary>
    public string? Password { get; set; }
}
