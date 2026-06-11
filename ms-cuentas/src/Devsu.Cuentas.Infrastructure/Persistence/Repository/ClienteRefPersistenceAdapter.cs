using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Domain.Model;
using Devsu.Cuentas.Infrastructure.Persistence.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Cuentas.Infrastructure.Persistence.Repository;

/// <summary>
/// Adaptador de salida que implementa <see cref="IClienteRefRepositoryPort"/> con
/// EF Core. El guardar es un <b>upsert</b> idempotente por clienteId (PK de negocio).
/// </summary>
public sealed class ClienteRefPersistenceAdapter(CuentasDbContext db) : IClienteRefRepositoryPort
{
    public async Task GuardarAsync(ClienteRef clienteRef, CancellationToken ct = default)
    {
        var entity = ClienteRefPersistenceMapper.ToEntity(clienteRef);
        var existe = await db.ClientesRef.AsNoTracking().AnyAsync(c => c.ClienteId == entity.ClienteId, ct);
        if (existe) db.ClientesRef.Update(entity);
        else db.ClientesRef.Add(entity);
        await db.SaveChangesAsync(ct);
    }

    public async Task<ClienteRef?> BuscarPorClienteIdAsync(string clienteId, CancellationToken ct = default)
    {
        var e = await db.ClientesRef.AsNoTracking().FirstOrDefaultAsync(c => c.ClienteId == clienteId, ct);
        return e is null ? null : ClienteRefPersistenceMapper.ToDomain(e);
    }

    public async Task EliminarPorClienteIdAsync(string clienteId, CancellationToken ct = default)
    {
        await db.ClientesRef.Where(c => c.ClienteId == clienteId).ExecuteDeleteAsync(ct);
    }
}
