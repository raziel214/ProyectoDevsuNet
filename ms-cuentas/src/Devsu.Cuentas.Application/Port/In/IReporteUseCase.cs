namespace Devsu.Cuentas.Application.Port.In;

/// <summary>
/// Caso de uso: generar el reporte "Estado de cuenta" (F4) por rango de fechas y
/// cliente. Una fila por movimiento de cada cuenta del cliente en el rango.
/// </summary>
public interface IReporteEstadoCuentaUseCase
{
    Task<IReadOnlyList<ReporteEstadoCuentaItem>> GenerarAsync(
        DateOnly fechaInicio, DateOnly fechaFin, string clienteId, CancellationToken ct = default);
}
