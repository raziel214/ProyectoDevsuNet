using Devsu.Clientes.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Clientes.Infrastructure.Persistence;

/// <summary>
/// Contexto de EF Core del microservicio de Clientes. Configura el mapeo de
/// <see cref="ClienteEntity"/> a la tabla <c>cliente</c> con Fluent API,
/// reproduciendo exactamente el esquema de <c>base-datos/BaseDatos.sql</c>.
/// </summary>
public class ClientesDbContext(DbContextOptions<ClientesDbContext> options) : DbContext(options)
{
    public DbSet<ClienteEntity> Clientes => Set<ClienteEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClienteEntity>(cliente =>
        {
            cliente.ToTable("cliente");

            cliente.HasKey(c => c.Id);
            cliente.Property(c => c.Id).HasColumnName("id").ValueGeneratedOnAdd();

            cliente.Property(c => c.Nombre)
                .HasColumnName("nombre").HasMaxLength(120).IsRequired();

            // genero -> VARCHAR(20), el enum se guarda como texto (MASCULINO/FEMENINO/OTRO)
            cliente.Property(c => c.Genero)
                .HasColumnName("genero").HasMaxLength(20).HasConversion<string>();

            cliente.Property(c => c.Edad).HasColumnName("edad");

            cliente.Property(c => c.Identificacion)
                .HasColumnName("identificacion").HasMaxLength(20).IsRequired();

            cliente.Property(c => c.Direccion)
                .HasColumnName("direccion").HasMaxLength(200);

            cliente.Property(c => c.Telefono)
                .HasColumnName("telefono").HasMaxLength(20);

            cliente.Property(c => c.ClienteId)
                .HasColumnName("cliente_id").HasMaxLength(50).IsRequired();

            cliente.Property(c => c.Contrasena)
                .HasColumnName("contrasena").HasMaxLength(255).IsRequired();

            cliente.Property(c => c.Estado)
                .HasColumnName("estado").IsRequired();

            cliente.HasIndex(c => c.Identificacion)
                .IsUnique().HasDatabaseName("uk_cliente_identificacion");

            cliente.HasIndex(c => c.ClienteId)
                .IsUnique().HasDatabaseName("uk_cliente_cliente_id");
        });
    }
}
