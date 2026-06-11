using Devsu.Cuentas.Application.Port.In;
using Devsu.Cuentas.Application.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Devsu.Cuentas.Application;

/// <summary>
/// Registro de la capa de aplicación en el contenedor de DI. Cada servicio
/// implementa varios casos de uso; se reexpone por cada puerto de entrada (ISP).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCuentasApplication(this IServiceCollection services)
    {
        // Cuentas
        services.AddScoped<CuentaService>();
        services.AddScoped<ICrearCuentaUseCase>(sp => sp.GetRequiredService<CuentaService>());
        services.AddScoped<IConsultarCuentaUseCase>(sp => sp.GetRequiredService<CuentaService>());
        services.AddScoped<IActualizarCuentaUseCase>(sp => sp.GetRequiredService<CuentaService>());
        services.AddScoped<ICambiarEstadoCuentaUseCase>(sp => sp.GetRequiredService<CuentaService>());

        // Movimientos
        services.AddScoped<MovimientoService>();
        services.AddScoped<IRegistrarMovimientoUseCase>(sp => sp.GetRequiredService<MovimientoService>());
        services.AddScoped<IConsultarMovimientoUseCase>(sp => sp.GetRequiredService<MovimientoService>());
        services.AddScoped<IActualizarMovimientoUseCase>(sp => sp.GetRequiredService<MovimientoService>());

        // Reporte (F4)
        services.AddScoped<IReporteEstadoCuentaUseCase, ReporteEstadoCuentaService>();

        // Sincronización de la réplica
        services.AddScoped<IClienteSyncUseCase, ClienteSyncService>();

        return services;
    }
}
