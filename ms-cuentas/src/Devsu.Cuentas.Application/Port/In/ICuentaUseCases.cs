using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Application.Port.In;

/// <summary>Caso de uso: crear una cuenta (F1).</summary>
public interface ICrearCuentaUseCase
{
    Task<Cuenta> CrearAsync(CrearCuentaCommand command, CancellationToken ct = default);
}

/// <summary>Caso de uso: consultar cuentas (F1).</summary>
public interface IConsultarCuentaUseCase
{
    Task<Cuenta> BuscarPorIdAsync(long id, CancellationToken ct = default);

    /// <summary>Lista todas las cuentas, o las de un cliente si se indica clienteId.</summary>
    Task<IReadOnlyList<Cuenta>> ListarAsync(string? clienteId = null, CancellationToken ct = default);
}

/// <summary>Caso de uso: actualizar los datos editables de una cuenta (tipo).</summary>
public interface IActualizarCuentaUseCase
{
    Task<Cuenta> ActualizarAsync(long id, ActualizarCuentaCommand command, CancellationToken ct = default);
}

/// <summary>
/// Caso de uso sensible: habilitar/inhabilitar una cuenta. Separado y auditable
/// (mismo criterio bancario que clientes).
/// </summary>
public interface ICambiarEstadoCuentaUseCase
{
    Task<Cuenta> CambiarEstadoAsync(long id, bool estado, CancellationToken ct = default);
}
