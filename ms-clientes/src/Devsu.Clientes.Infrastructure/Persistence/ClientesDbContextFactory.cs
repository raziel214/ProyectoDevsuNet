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
        // Solo para diseño (dotnet-ef). Generar/scriptear migraciones NO se conecta
        // a la BD, por eso alcanza un placeholder sin credenciales reales. Para
        // operaciones que sí conectan (database update), definir DESIGN_CONNECTION_STRING.
        var connectionString = Environment.GetEnvironmentVariable("DESIGN_CONNECTION_STRING")
            ?? "Host=localhost;Database=design;Username=design;Password=design";

        var options = new DbContextOptionsBuilder<ClientesDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ClientesDbContext(options);
    }
}
