using Devsu.Clientes.Application.Port.Out;
using Devsu.Clientes.Domain.Model;
using Devsu.Clientes.Infrastructure.Persistence.Mapper;
using Microsoft.EntityFrameworkCore;

namespace Devsu.Clientes.Infrastructure.Persistence.Repository;

/// <summary>
/// Adaptador de salida que implementa <see cref="IClienteRepositoryPort"/> usando
/// EF Core.
///
/// <para>Es el "puente" entre la aplicación (que solo conoce el puerto) y la
/// infraestructura (EF Core + entidad). Convierte dominio ↔ entidad con el
/// mapper. Patrón <b>Adapter</b>, base de Ports &amp; Adapters.</para>
///
/// <para>Las consultas usan <c>AsNoTracking</c> (lecturas sin seguimiento): el
/// dominio es inmutable y la actualización reconstruye el agregado, por lo que no
/// se necesita el change tracker para leer.</para>
/// </summary>
public sealed class ClientePersistenceAdapter(ClientesDbContext db) : IClienteRepositoryPort
{
    public async Task<Cliente> GuardarAsync(Cliente cliente, CancellationToken ct = default)
    {
        var entity = ClientePersistenceMapper.ToEntity(cliente);

        if (entity.Id == 0)
            db.Clientes.Add(entity);
        else
            db.Clientes.Update(entity);

        await db.SaveChangesAsync(ct);
        return ClientePersistenceMapper.ToDomain(entity);
    }

    public async Task<Cliente?> BuscarPorIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        return entity is null ? null : ClientePersistenceMapper.ToDomain(entity);
    }

    public async Task<Cliente?> BuscarPorClienteIdAsync(string clienteId, CancellationToken ct = default)
    {
        var entity = await db.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClienteId == clienteId, ct);
        return entity is null ? null : ClientePersistenceMapper.ToDomain(entity);
    }

    public async Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken ct = default)
    {
        var entities = await db.Clientes.AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync(ct);
        return entities.Select(ClientePersistenceMapper.ToDomain).ToList();
    }

    public async Task EliminarPorIdAsync(long id, CancellationToken ct = default)
    {
        await db.Clientes
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync(ct);
    }

    public Task<bool> ExistePorClienteIdAsync(string clienteId, CancellationToken ct = default)
        => db.Clientes.AsNoTracking().AnyAsync(c => c.ClienteId == clienteId, ct);

    public Task<bool> ExistePorIdentificacionAsync(string identificacion, CancellationToken ct = default)
        => db.Clientes.AsNoTracking().AnyAsync(c => c.Identificacion == identificacion, ct);
}
