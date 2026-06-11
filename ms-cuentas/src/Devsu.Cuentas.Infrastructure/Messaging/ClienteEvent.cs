namespace Devsu.Cuentas.Infrastructure.Messaging;

/// <summary>
/// Evento de cliente recibido desde ms-clientes (contrato async-eventos-rabbitmq).
/// En DELETED solo se garantizan EventType, ClienteId y Timestamp.
/// </summary>
public record ClienteEvent(
    string EventType,
    string ClienteId,
    string? Nombre,
    string? Identificacion,
    bool? Estado,
    string? Timestamp
);
