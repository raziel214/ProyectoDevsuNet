using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Devsu.Clientes.IntegrationTests;

/// <summary>
/// Factory de integración: levanta Postgres y RabbitMQ efímeros (Testcontainers),
/// deshabilita Vault y reemplaza la seguridad JWT por un esquema de prueba.
///
/// <para>La configuración que <c>Program</c> lee ANTES de construir el host
/// (Vault, cadena de conexión) se inyecta por variables de entorno, ya que el
/// override por <c>ConfigureAppConfiguration</c> se aplicaría demasiado tarde.</para>
/// </summary>
public sealed class ClientesApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string Db = "devsu_clientes_test";

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase(Db)
        .WithUsername("devsu")
        .WithPassword("devsu")
        .Build();

    private readonly RabbitMqContainer _rabbit = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management-alpine")
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await _rabbit.StartAsync();

        // Config que Program lee antes de Build(): via variables de entorno.
        Environment.SetEnvironmentVariable("Vault__Enabled", "false");
        Environment.SetEnvironmentVariable("MigrateOnStartup", "true");
        Environment.SetEnvironmentVariable("Database__Host", _postgres.Hostname);
        Environment.SetEnvironmentVariable("Database__Port", _postgres.GetMappedPublicPort(5432).ToString());
        Environment.SetEnvironmentVariable("Database__Name", Db);
        Environment.SetEnvironmentVariable("Database__Username", "devsu");
        Environment.SetEnvironmentVariable("Database__Password", "devsu");
        Environment.SetEnvironmentVariable("RabbitMq__Host", _rabbit.Hostname);
        Environment.SetEnvironmentVariable("RabbitMq__Port", _rabbit.GetMappedPublicPort(5672).ToString());
        Environment.SetEnvironmentVariable("RabbitMq__Username", "guest");
        Environment.SetEnvironmentVariable("RabbitMq__Password", "guest");
        Environment.SetEnvironmentVariable("RabbitMq__VirtualHost", "/");
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await _rabbit.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                options.DefaultChallengeScheme = TestAuthHandler.Scheme;
                options.DefaultScheme = TestAuthHandler.Scheme;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });
        });
    }
}
