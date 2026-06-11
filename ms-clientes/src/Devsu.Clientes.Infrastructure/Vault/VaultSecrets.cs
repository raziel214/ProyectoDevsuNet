namespace Devsu.Clientes.Infrastructure.Vault;

/// <summary>
/// Credenciales resueltas desde Vault (o desde configuración si Vault está
/// deshabilitado). Las claves en Vault conservan el nombre sembrado por la infra
/// compartida (<c>spring.datasource.*</c>, <c>spring.rabbitmq.*</c>).
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
