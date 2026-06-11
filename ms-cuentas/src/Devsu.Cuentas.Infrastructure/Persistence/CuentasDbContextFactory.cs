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
        var connectionString = Environment.GetEnvironmentVariable("DESIGN_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=devsu_cuentas_net;Username=devsu;Password=devsu";

        var options = new DbContextOptionsBuilder<CuentasDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new CuentasDbContext(options);
    }
}
