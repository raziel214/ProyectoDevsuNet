namespace Devsu.Clientes.Infrastructure.Messaging;

/// <summary>
/// Evento de dominio de Cliente que se publica hacia otros microservicios.
///
/// <para>DTO de mensajería (el formato del mensaje en el bus). Su estructura debe
/// coincidir con la que espera el consumidor (ms-cuentas).</para>
/// </summary>
/// <param name="EventType">CREATED, UPDATED o DELETED.</param>
/// <param name="ClienteId">Clave de negocio del cliente.</param>
/// <param name="Nombre">Nombre del cliente.</param>
/// <param name="Identificacion">Documento de identidad.</param>
/// <param name="Estado">Estado del cliente.</param>
/// <param name="Timestamp">Momento del evento (ISO-8601).</param>
public record ClienteEvent(
    string EventType,
    string ClienteId,
    string? Nombre,
    string? Identificacion,
    bool? Estado,
    string Timestamp
);
