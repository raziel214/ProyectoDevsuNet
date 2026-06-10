using Devsu.Clientes.Application.Port.In;
using Devsu.Clientes.Application.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Devsu.Clientes.Application;

/// <summary>
/// Registro de la capa de aplicación en el contenedor de DI.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddClientesApplication(this IServiceCollection services)
    {
        // Un unico servicio implementa los cuatro casos de uso; se reexpone por
        // cada puerto de entrada para respetar la segregacion de interfaces (ISP).
        services.AddScoped<ClienteService>();
        services.AddScoped<ICrearClienteUseCase>(sp => sp.GetRequiredService<ClienteService>());
        services.AddScoped<IConsultarClienteUseCase>(sp => sp.GetRequiredService<ClienteService>());
        services.AddScoped<IActualizarClienteUseCase>(sp => sp.GetRequiredService<ClienteService>());
        services.AddScoped<ICambiarEstadoClienteUseCase>(sp => sp.GetRequiredService<ClienteService>());
        services.AddScoped<ICambiarContrasenaClienteUseCase>(sp => sp.GetRequiredService<ClienteService>());
        services.AddScoped<IEliminarClienteUseCase>(sp => sp.GetRequiredService<ClienteService>());

        return services;
    }
}
