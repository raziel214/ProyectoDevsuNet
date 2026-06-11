using Devsu.Cuentas.Application.Port.Out;

namespace Devsu.Cuentas.Infrastructure.Persistence;

/// <summary>
/// Implementación de <see cref="IUnitOfWork"/> con una transacción de EF Core.
/// Las operaciones dentro de la acción (cada una con su SaveChanges) se enlistan
/// en la misma transacción y se confirman juntas, o se revierte todo.
/// </summary>
public sealed class EfUnitOfWork(CuentasDbContext db) : IUnitOfWork
{
    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            await action(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
