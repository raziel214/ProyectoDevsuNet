using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Infrastructure.Messaging;
using Devsu.Cuentas.Infrastructure.Persistence;
using Devsu.Cuentas.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Devsu.Cuentas.Infrastructure;

/// <summary>
/// Registro de la capa de infraestructura (adaptadores de salida y el consumidor
/// de entrada) en el contenedor de DI.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCuentasInfrastructure(
        this IServiceCollection services,
        string connectionString,
        Action<RabbitMqOptions> configureRabbit)
    {
        services.AddDbContext<CuentasDbContext>(options => options.UseNpgsql(connectionString));

        services.Configure(configureRabbit);

        // Adaptadores de persistencia
        services.AddScoped<ICuentaRepositoryPort, CuentaPersistenceAdapter>();
        services.AddScoped<IMovimientoRepositoryPort, MovimientoPersistenceAdapter>();
        services.AddScoped<IClienteRefRepositoryPort, ClienteRefPersistenceAdapter>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Consumidor de eventos de cliente (adaptador de entrada / hosted service)
        services.AddHostedService<ClienteSyncConsumer>();

        return services;
    }
}
