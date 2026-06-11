namespace Devsu.Cuentas.Application.Port.Out;

/// <summary>
/// Puerto de salida para ejecutar varias operaciones de persistencia de forma
/// atómica (una transacción).
///
/// <para>Necesario al registrar un movimiento (F2): actualizar el saldo de la
/// cuenta y crear el asiento del ledger deben confirmarse juntos o no
/// confirmarse. La aplicación expresa "hacé esto atómico" sin conocer EF Core.</para>
/// </summary>
public interface IUnitOfWork
{
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
}
