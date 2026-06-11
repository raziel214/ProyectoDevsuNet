using Devsu.Cuentas.Domain.Model;
using Devsu.Cuentas.Infrastructure.Persistence.Entity;

namespace Devsu.Cuentas.Infrastructure.Persistence.Mapper;

/// <summary>Traduce entre el agregado <see cref="Cuenta"/> y su entidad EF.</summary>
public static class CuentaPersistenceMapper
{
    public static CuentaEntity ToEntity(Cuenta c) => new()
    {
        Id = c.Id ?? 0,
        NumeroCuenta = c.NumeroCuenta,
        TipoCuenta = c.TipoCuenta,
        SaldoInicial = c.SaldoInicial,
        SaldoDisponible = c.SaldoDisponible,
        Estado = c.Estado,
        ClienteId = c.ClienteId
    };

    public static Cuenta ToDomain(CuentaEntity e) => new()
    {
        Id = e.Id,
        NumeroCuenta = e.NumeroCuenta,
        TipoCuenta = e.TipoCuenta,
        SaldoInicial = e.SaldoInicial,
        SaldoDisponible = e.SaldoDisponible,
        Estado = e.Estado,
        ClienteId = e.ClienteId
    };
}

/// <summary>Traduce entre el agregado <see cref="Movimiento"/> y su entidad EF.</summary>
public static class MovimientoPersistenceMapper
{
    public static MovimientoEntity ToEntity(Movimiento m) => new()
    {
        Id = m.Id ?? 0,
        Fecha = m.Fecha,
        TipoMovimiento = m.TipoMovimiento,
        Valor = m.Valor,
        Saldo = m.Saldo,
        CuentaId = m.CuentaId,
        Descripcion = m.Descripcion
    };

    public static Movimiento ToDomain(MovimientoEntity e) => new()
    {
        Id = e.Id,
        Fecha = e.Fecha,
        TipoMovimiento = e.TipoMovimiento,
        Valor = e.Valor,
        Saldo = e.Saldo,
        CuentaId = e.CuentaId,
        Descripcion = e.Descripcion
    };
}

/// <summary>Traduce entre el read-model <see cref="ClienteRef"/> y su entidad EF.</summary>
public static class ClienteRefPersistenceMapper
{
    public static ClienteRefEntity ToEntity(ClienteRef c) => new()
    {
        ClienteId = c.ClienteId,
        Nombre = c.Nombre,
        Identificacion = c.Identificacion,
        Estado = c.Estado,
        ActualizadoEn = c.ActualizadoEn
    };

    public static ClienteRef ToDomain(ClienteRefEntity e) => new()
    {
        ClienteId = e.ClienteId,
        Nombre = e.Nombre,
        Identificacion = e.Identificacion,
        Estado = e.Estado,
        ActualizadoEn = e.ActualizadoEn
    };
}
