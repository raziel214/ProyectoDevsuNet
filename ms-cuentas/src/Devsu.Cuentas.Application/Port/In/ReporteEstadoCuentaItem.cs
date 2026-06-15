using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Application.Port.In;

/// <summary>
/// Fila del reporte de estado de cuenta (F4). Modelo de aplicación con nombres
/// normales; el adaptador web lo mapea a las claves JSON exactas del enunciado
/// ("Fecha", "Cliente", "Numero Cuenta", "Saldo Inicial", etc.).
///
/// <para>Cada fila corresponde a un movimiento: "Saldo Inicial" es el saldo
/// inicial de la cuenta y "Saldo Disponible" es el saldo resultante tras ese
/// movimiento.</para>
/// </summary>
public record ReporteEstadoCuentaItem(
    DateTime Fecha,
    string Cliente,
    string NumeroCuenta,
    TipoCuenta Tipo,
    decimal SaldoInicial,
    bool Estado,
    decimal Movimiento,
    decimal SaldoDisponible
);
