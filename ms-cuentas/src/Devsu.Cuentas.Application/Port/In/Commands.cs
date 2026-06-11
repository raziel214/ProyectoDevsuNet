using Devsu.Cuentas.Domain.Model;

namespace Devsu.Cuentas.Application.Port.In;

/// <summary>Datos para crear una cuenta (F1). El saldo disponible inicial = saldo inicial.</summary>
public record CrearCuentaCommand(
    string NumeroCuenta,
    TipoCuenta TipoCuenta,
    decimal SaldoInicial,
    bool Estado,
    string ClienteId
);

/// <summary>
/// Datos editables de una cuenta. Solo el tipo: el estado es una operación
/// sensible aparte y los saldos/identidad no se editan a mano.
/// </summary>
public record ActualizarCuentaCommand(
    TipoCuenta TipoCuenta
);

/// <summary>Datos para registrar un movimiento (F2/F3). Valor con signo, distinto de cero.</summary>
public record RegistrarMovimientoCommand(
    string NumeroCuenta,
    decimal Valor
);
