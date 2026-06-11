namespace Devsu.Cuentas.Domain.Exceptions;

/// <summary>
/// Se lanza cuando un retiro deja el saldo disponible en negativo (F3).
///
/// <para>El mensaje es exactamente <c>"Saldo no disponible"</c> (requisito del
/// enunciado). El adaptador web la traduce a un HTTP 422.</para>
/// </summary>
public sealed class SaldoNoDisponibleException() : Exception("Saldo no disponible");
