using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Devsu.Clientes.Infrastructure.Persistence;

/// <summary>
/// Factory de diseño usado SOLO por las herramientas de EF Core (dotnet-ef) para
/// generar y aplicar migraciones sin necesidad de arrancar el host del Api.
///
/// <para>La cadena de conexión aquí es de diseño (apunta a la BD local). En
/// ejecución, el <c>DbContext</c> se configura vía DI desde el Api con la cadena
/// real (variables de entorno / Vault).</para>
/// </summary>
public sealed class ClientesDbContextFactory : IDesignTimeDbContextFactory<ClientesDbContext>
{
    public ClientesDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("DESIGN_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=devsu_clientes;Username=devsu;Password=devsu";

        var options = new DbContextOptionsBuilder<ClientesDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ClientesDbContext(options);
    }
}
