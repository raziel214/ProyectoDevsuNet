using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Application.Port.Out;

/// <summary>
/// Puerto de salida para la persistencia de <see cref="Cuenta"/>. La aplicación
/// define qué necesita en términos del dominio; la implementación EF Core vive en
/// Infrastructure (inversión de dependencias).
/// </summary>
public interface ICuentaRepositoryPort
{
    Task<Cuenta> GuardarAsync(Cuenta cuenta, CancellationToken ct = default);
    Task<Cuenta?> BuscarPorIdAsync(long id, CancellationToken ct = default);
    Task<Cuenta?> BuscarPorNumeroCuentaAsync(string numeroCuenta, CancellationToken ct = default);
    Task<IReadOnlyList<Cuenta>> ListarAsync(CancellationToken ct = default);

    /// <summary>Lista las cuentas de un cliente (usado por el reporte F4).</summary>
    Task<IReadOnlyList<Cuenta>> ListarPorClienteIdAsync(string clienteId, CancellationToken ct = default);

    Task<bool> ExistePorNumeroCuentaAsync(string numeroCuenta, CancellationToken ct = default);
}
