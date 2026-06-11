using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Application.Port.Out;

/// <summary>
/// Puerto de salida para la persistencia de <see cref="Movimiento"/>, incluyendo
/// la consulta por cuenta y rango de fechas para el reporte (F4).
/// </summary>
public interface IMovimientoRepositoryPort
{
    Task<Movimiento> GuardarAsync(Movimiento movimiento, CancellationToken ct = default);
    Task<Movimiento?> BuscarPorIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Movimiento>> ListarAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Movimiento>> ListarPorCuentaIdAsync(long cuentaId, CancellationToken ct = default);

    /// <summary>Movimientos de una cuenta dentro de un rango de fechas (reporte F4).</summary>
    Task<IReadOnlyList<Movimiento>> BuscarPorCuentaIdYRangoAsync(
        long cuentaId, DateTime desde, DateTime hasta, CancellationToken ct = default);
}
