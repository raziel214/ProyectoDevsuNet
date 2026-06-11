using Devsu.Cuentas.Api.Web.Dto;
using Devsu.Cuentas.Application.Port.In;
using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Api.Web.Mapper;

/// <summary>Traduce entre los DTOs de cuenta y los commands/dominio.</summary>
public static class CuentaWebMapper
{
    public static CrearCuentaCommand ToCrearCommand(CuentaRequest r) => new(
        r.NumeroCuenta, r.TipoCuenta!.Value, r.SaldoInicial!.Value, r.Estado!.Value, r.ClienteId);

    public static ActualizarCuentaCommand ToActualizarCommand(ActualizarCuentaRequest r) => new(
        r.TipoCuenta!.Value);

    public static CuentaResponse ToResponse(Cuenta c) => new(
        c.Id, c.NumeroCuenta, c.TipoCuenta, c.SaldoInicial, c.SaldoDisponible, c.Estado, c.ClienteId);
}

/// <summary>Traduce entre los DTOs de movimiento y los commands/dominio.</summary>
public static class MovimientoWebMapper
{
    public static RegistrarMovimientoCommand ToRegistrarCommand(MovimientoRequest r) => new(
        r.NumeroCuenta, r.Valor!.Value);

    /// <summary>El numeroCuenta se resuelve en el controller (el dominio guarda cuentaId).</summary>
    public static MovimientoResponse ToResponse(Movimiento m, string numeroCuenta) => new(
        m.Id, m.Fecha, m.TipoMovimiento, m.Valor, m.Saldo, numeroCuenta, m.Descripcion);
}

/// <summary>Traduce el modelo del reporte a las filas con las claves JSON exactas (F4).</summary>
public static class ReporteWebMapper
{
    public static ReporteEstadoCuentaRow ToRow(ReporteEstadoCuentaItem i) => new()
    {
        Fecha = $"{i.Fecha.Day}/{i.Fecha.Month}/{i.Fecha.Year}",
        Cliente = i.Cliente,
        NumeroCuenta = i.NumeroCuenta,
        Tipo = Capitalizar(i.Tipo),
        SaldoInicial = i.SaldoInicial,
        Estado = i.Estado,
        Movimiento = i.Movimiento,
        SaldoDisponible = i.SaldoDisponible
    };

    private static string Capitalizar(TipoCuenta tipo) => tipo switch
    {
        TipoCuenta.AHORRO => "Ahorro",
        TipoCuenta.CORRIENTE => "Corriente",
        _ => tipo.ToString()
    };
}
