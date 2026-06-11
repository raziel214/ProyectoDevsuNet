using Devsu.Cuentas.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Cuentas.Infrastructure.Persistence;

/// <summary>
/// Contexto EF Core del microservicio de Cuentas. El mapeo Fluent reproduce el
/// esquema de <c>base-datos/BaseDatos.sql</c> (tablas cuenta, movimiento,
/// cliente_ref).
/// </summary>
public class CuentasDbContext(DbContextOptions<CuentasDbContext> options) : DbContext(options)
{
    public DbSet<CuentaEntity> Cuentas => Set<CuentaEntity>();
    public DbSet<MovimientoEntity> Movimientos => Set<MovimientoEntity>();
    public DbSet<ClienteRefEntity> ClientesRef => Set<ClienteRefEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CuentaEntity>(cuenta =>
        {
            cuenta.ToTable("cuenta");
            cuenta.HasKey(c => c.Id);
            cuenta.Property(c => c.Id).HasColumnName("id").ValueGeneratedOnAdd();
            cuenta.Property(c => c.NumeroCuenta).HasColumnName("numero_cuenta").HasMaxLength(20).IsRequired();
            cuenta.Property(c => c.TipoCuenta).HasColumnName("tipo_cuenta").HasMaxLength(20).HasConversion<string>().IsRequired();
            cuenta.Property(c => c.SaldoInicial).HasColumnName("saldo_inicial").HasPrecision(19, 2).IsRequired();
            cuenta.Property(c => c.SaldoDisponible).HasColumnName("saldo_disponible").HasPrecision(19, 2).IsRequired();
            cuenta.Property(c => c.Estado).HasColumnName("estado").IsRequired();
            cuenta.Property(c => c.ClienteId).HasColumnName("cliente_id").HasMaxLength(50).IsRequired();
            cuenta.HasIndex(c => c.NumeroCuenta).IsUnique().HasDatabaseName("uk_cuenta_numero");
        });

        modelBuilder.Entity<MovimientoEntity>(mov =>
        {
            mov.ToTable("movimiento");
            mov.HasKey(m => m.Id);
            mov.Property(m => m.Id).HasColumnName("id").ValueGeneratedOnAdd();
            mov.Property(m => m.Fecha).HasColumnName("fecha").HasColumnType("timestamp without time zone").IsRequired();
            mov.Property(m => m.TipoMovimiento).HasColumnName("tipo_movimiento").HasMaxLength(20).HasConversion<string>().IsRequired();
            mov.Property(m => m.Valor).HasColumnName("valor").HasPrecision(19, 2).IsRequired();
            mov.Property(m => m.Saldo).HasColumnName("saldo").HasPrecision(19, 2).IsRequired();
            mov.Property(m => m.CuentaId).HasColumnName("cuenta_id").IsRequired();
            mov.Property(m => m.Descripcion).HasColumnName("descripcion").HasMaxLength(255);
            mov.HasIndex(m => m.CuentaId).HasDatabaseName("idx_movimiento_cuenta");
            mov.HasOne<CuentaEntity>()
                .WithMany()
                .HasForeignKey(m => m.CuentaId)
                .HasConstraintName("fk_movimiento_cuenta");
        });

        modelBuilder.Entity<ClienteRefEntity>(cli =>
        {
            cli.ToTable("cliente_ref");
            cli.HasKey(c => c.ClienteId);
            cli.Property(c => c.ClienteId).HasColumnName("cliente_id").HasMaxLength(50);
            cli.Property(c => c.Nombre).HasColumnName("nombre").HasMaxLength(120).IsRequired();
            cli.Property(c => c.Identificacion).HasColumnName("identificacion").HasMaxLength(20);
            cli.Property(c => c.Estado).HasColumnName("estado").IsRequired();
            cli.Property(c => c.ActualizadoEn).HasColumnName("actualizado_en").HasColumnType("timestamp without time zone").IsRequired();
        });
    }
}
