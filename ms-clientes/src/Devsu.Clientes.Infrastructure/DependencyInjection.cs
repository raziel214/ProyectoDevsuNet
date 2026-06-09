using Devsu.Clientes.Application.Port.Out;
using Devsu.Clientes.Infrastructure.Messaging;
using Devsu.Clientes.Infrastructure.Persistence;
using Devsu.Clientes.Infrastructure.Persistence.Repository;
using Devsu.Clientes.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Devsu.Clientes.Infrastructure;

/// <summary>
/// Registro de la capa de infraestructura (adaptadores de salida) en el
/// contenedor de DI. Aquí se enlazan los puertos del dominio con sus
/// implementaciones concretas (EF Core, BCrypt, RabbitMQ).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddClientesInfrastructure(
        this IServiceCollection services,
        string connectionString,
        Action<RabbitMqOptions> configureRabbit)
    {
        services.AddDbContext<ClientesDbContext>(options => options.UseNpgsql(connectionString));

        services.Configure(configureRabbit);

        services.AddScoped<IClienteRepositoryPort, ClientePersistenceAdapter>();
        services.AddSingleton<IPasswordHasherPort, BCryptPasswordHasherAdapter>();
        services.AddSingleton<IClienteEventPublisherPort, ClienteEventPublisherAdapter>();

        return services;
    }
}
