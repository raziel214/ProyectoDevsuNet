using Devsu.Clientes.Domain.Model;

namespace Devsu.Clientes.Application.Port.Out;

/// <summary>
/// Puerto de salida para publicar eventos de dominio de Cliente hacia otros
/// microservicios (comunicación asíncrona).
///
/// <para>La aplicación expresa <i>qué</i> evento ocurrió en términos del dominio;
/// el adaptador en <c>Infrastructure.Messaging</c> lo traduce a un mensaje y lo
/// publica en RabbitMQ. El dominio/aplicación no conoce RabbitMQ.</para>
/// </summary>
public interface IClienteEventPublisherPort
{
    /// <summary>Publica que un cliente fue creado (routing key cliente.created).</summary>
    Task PublicarCreadoAsync(Cliente cliente, CancellationToken ct = default);

    /// <summary>Publica que un cliente fue actualizado (routing key cliente.updated).</summary>
    Task PublicarActualizadoAsync(Cliente cliente, CancellationToken ct = default);

    /// <summary>Publica que un cliente fue eliminado (routing key cliente.deleted).</summary>
    Task PublicarEliminadoAsync(string clienteId, CancellationToken ct = default);
}
