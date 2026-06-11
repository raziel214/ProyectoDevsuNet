using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Domain.Model;
using Devsu.Cuentas.Infrastructure.Persistence.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Cuentas.Infrastructure.Persistence.Repository;

/// <summary>Adaptador de salida que implementa <see cref="IMovimientoRepositoryPort"/> con EF Core.</summary>
public sealed class MovimientoPersistenceAdapter(CuentasDbContext db) : IMovimientoRepositoryPort
{
    public async Task<Movimiento> GuardarAsync(Movimiento movimiento, CancellationToken ct = default)
    {
        var entity = MovimientoPersistenceMapper.ToEntity(movimiento);
        if (entity.Id == 0) db.Movimientos.Add(entity);
        else db.Movimientos.Update(entity);
        await db.SaveChangesAsync(ct);
        return MovimientoPersistenceMapper.ToDomain(entity);
    }

    public async Task<Movimiento?> BuscarPorIdAsync(long id, CancellationToken ct = default)
    {
        var e = await db.Movimientos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);
        return e is null ? null : MovimientoPersistenceMapper.ToDomain(e);
    }

    public async Task<IReadOnlyList<Movimiento>> ListarAsync(CancellationToken ct = default)
    {
        var list = await db.Movimientos.AsNoTracking().OrderBy(m => m.Id).ToListAsync(ct);
        return list.Select(MovimientoPersistenceMapper.ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Movimiento>> ListarPorCuentaIdAsync(long cuentaId, CancellationToken ct = default)
    {
        var list = await db.Movimientos.AsNoTracking()
            .Where(m => m.CuentaId == cuentaId).OrderBy(m => m.Fecha).ToListAsync(ct);
        return list.Select(MovimientoPersistenceMapper.ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Movimiento>> BuscarPorCuentaIdYRangoAsync(
        long cuentaId, DateTime desde, DateTime hasta, CancellationToken ct = default)
    {
        var list = await db.Movimientos.AsNoTracking()
            .Where(m => m.CuentaId == cuentaId && m.Fecha >= desde && m.Fecha <= hasta)
            .OrderBy(m => m.Fecha).ToListAsync(ct);
        return list.Select(MovimientoPersistenceMapper.ToDomain).ToList();
    }
}
