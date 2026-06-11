using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Devsu.Cuentas.Infrastructure.Persistence;

/// <summary>
/// Factory de diseño usado por las herramientas de EF Core (dotnet-ef) para
/// generar y aplicar migraciones sin arrancar el host del Api.
/// </summary>
public sealed class CuentasDbContextFactory : IDesignTimeDbContextFactory<CuentasDbContext>
{
    public CuentasDbContext CreateDbContext(string[] args)
    {
        // Solo para diseño (dotnet-ef). Generar/scriptear migraciones NO se conecta
        // a la BD, por eso alcanza un placeholder sin credenciales reales. Para
        // operaciones que sí conectan (database update), definir DESIGN_CONNECTION_STRING.
        var connectionString = Environment.GetEnvironmentVariable("DESIGN_CONNECTION_STRING")
            ?? "Host=localhost;Database=design;Username=design;Password=design";

        var options = new DbContextOptionsBuilder<CuentasDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new CuentasDbContext(options);
    }
}
