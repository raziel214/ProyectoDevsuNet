using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Domain.Model;
using Devsu.Cuentas.Infrastructure.Persistence.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Cuentas.Infrastructure.Persistence.Repository;

/// <summary>Adaptador de salida que implementa <see cref="ICuentaRepositoryPort"/> con EF Core.</summary>
public sealed class CuentaPersistenceAdapter(CuentasDbContext db) : ICuentaRepositoryPort
{
    public async Task<Cuenta> GuardarAsync(Cuenta cuenta, CancellationToken ct = default)
    {
        var entity = CuentaPersistenceMapper.ToEntity(cuenta);
        if (entity.Id == 0) db.Cuentas.Add(entity);
        else db.Cuentas.Update(entity);
        await db.SaveChangesAsync(ct);
        return CuentaPersistenceMapper.ToDomain(entity);
    }

    public async Task<Cuenta?> BuscarPorIdAsync(long id, CancellationToken ct = default)
    {
        var e = await db.Cuentas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
        return e is null ? null : CuentaPersistenceMapper.ToDomain(e);
    }

    public async Task<Cuenta?> BuscarPorNumeroCuentaAsync(string numeroCuenta, CancellationToken ct = default)
    {
        var e = await db.Cuentas.AsNoTracking().FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta, ct);
        return e is null ? null : CuentaPersistenceMapper.ToDomain(e);
    }

    public async Task<IReadOnlyList<Cuenta>> ListarAsync(CancellationToken ct = default)
    {
        var list = await db.Cuentas.AsNoTracking().OrderBy(c => c.Id).ToListAsync(ct);
        return list.Select(CuentaPersistenceMapper.ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Cuenta>> ListarPorClienteIdAsync(string clienteId, CancellationToken ct = default)
    {
        var list = await db.Cuentas.AsNoTracking()
            .Where(c => c.ClienteId == clienteId).OrderBy(c => c.Id).ToListAsync(ct);
        return list.Select(CuentaPersistenceMapper.ToDomain).ToList();
    }

    public Task<bool> ExistePorNumeroCuentaAsync(string numeroCuenta, CancellationToken ct = default)
        => db.Cuentas.AsNoTracking().AnyAsync(c => c.NumeroCuenta == numeroCuenta, ct);
}
