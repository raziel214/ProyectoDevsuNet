using Devsu.Cuentas.Application.Port.In;
using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Domain.Exceptions;

namespace Devsu.Cuentas.Application.Service;

/// <summary>
/// Servicio del reporte "Estado de cuenta" (F4): para un cliente y un rango de
/// fechas, devuelve una fila por cada movimiento de cada cuenta del cliente.
///
/// <para>Resuelve el nombre del cliente desde la réplica local (cliente_ref),
/// sincronizada de forma asíncrona desde ms-clientes.</para>
/// </summary>
public sealed class ReporteEstadoCuentaService(
    IClienteRefRepositoryPort clienteRefRepository,
    ICuentaRepositoryPort cuentaRepository,
    IMovimientoRepositoryPort movimientoRepository)
    : IReporteEstadoCuentaUseCase
{
    public async Task<IReadOnlyList<ReporteEstadoCuentaItem>> GenerarAsync(
        DateOnly fechaInicio, DateOnly fechaFin, string clienteId, CancellationToken ct = default)
    {
        var cliente = await clienteRefRepository.BuscarPorClienteIdAsync(clienteId, ct)
                      ?? throw new ClienteNoEncontradoException(clienteId);

        var desde = fechaInicio.ToDateTime(TimeOnly.MinValue);
        var hasta = fechaFin.ToDateTime(TimeOnly.MaxValue); // fin de día inclusive

        var cuentas = await cuentaRepository.ListarPorClienteIdAsync(clienteId, ct);

        var filas = new List<ReporteEstadoCuentaItem>();
        foreach (var cuenta in cuentas)
        {
            var movimientos = await movimientoRepository.BuscarPorCuentaIdYRangoAsync(
                cuenta.Id!.Value, desde, hasta, ct);

            filas.AddRange(movimientos.Select(m => new ReporteEstadoCuentaItem(
                Fecha: m.Fecha,
                Cliente: cliente.Nombre,
                NumeroCuenta: cuenta.NumeroCuenta,
                Tipo: cuenta.TipoCuenta,
                SaldoInicial: cuenta.SaldoInicial,
                Estado: cuenta.Estado,
                Movimiento: m.Valor,
                SaldoDisponible: m.Saldo)));
        }

        return filas.OrderBy(f => f.Fecha).ToList();
    }
}
