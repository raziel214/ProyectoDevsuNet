namespace Devsu.Cuentas.Infrastructure.Vault;

/// <summary>
/// Credenciales resueltas desde Vault (o configuración si está deshabilitado).
/// Las claves conservan el nombre sembrado por la infra compartida.
/// </summary>
public sealed record VaultSecrets(
    string DbUsername,
    string DbPassword,
    string? RabbitUsername,
    string? RabbitPassword)
{
    public const string KeyDbUsername = "spring.datasource.username";
    public const string KeyDbPassword = "spring.datasource.password";
    public const string KeyRabbitUsername = "spring.rabbitmq.username";
    public const string KeyRabbitPassword = "spring.rabbitmq.password";
}
