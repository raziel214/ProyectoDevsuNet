using Devsu.Clientes.Api.Web.Dto;
using Devsu.Clientes.Api.Web.Mapper;
using Devsu.Clientes.Application.Port.In;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.Clientes.Api.Web.Controllers;

/// <summary>
/// Adaptador de entrada (REST) del CRUD de Cliente (F1).
///
/// <para>Depende de las interfaces de caso de uso (puertos in), no de la
/// implementación. Traduce DTO ↔ command con el mapper web.</para>
/// </summary>
[ApiController]
[Route("api/clientes")]
[Produces("application/json")]
public sealed class ClientesController(
    ICrearClienteUseCase crearCliente,
    IConsultarClienteUseCase consultarCliente,
    IActualizarClienteUseCase actualizarCliente,
    IEliminarClienteUseCase eliminarCliente) : ControllerBase
{
    /// <summary>Crear cliente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteResponse>> Crear([FromBody] ClienteRequest request, CancellationToken ct)
    {
        var cliente = await crearCliente.CrearAsync(ClienteWebMapper.ToCrearCommand(request), ct);
        var response = ClienteWebMapper.ToResponse(cliente);
        return CreatedAtAction(nameof(Obtener), new { id = response.Id }, response);
    }

    /// <summary>Listar clientes.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClienteResponse>>> Listar(CancellationToken ct)
    {
        var clientes = await consultarCliente.ListarAsync(ct);
        return Ok(clientes.Select(ClienteWebMapper.ToResponse));
    }

    /// <summary>Obtener cliente por id.</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> Obtener(long id, CancellationToken ct)
    {
        var cliente = await consultarCliente.BuscarPorIdAsync(id, ct);
        return Ok(ClienteWebMapper.ToResponse(cliente));
    }

    /// <summary>Actualizar cliente.</summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> Actualizar(long id, [FromBody] ClienteRequest request, CancellationToken ct)
    {
        var cliente = await actualizarCliente.ActualizarAsync(id, ClienteWebMapper.ToActualizarCommand(request), ct);
        return Ok(ClienteWebMapper.ToResponse(cliente));
    }

    /// <summary>Eliminar cliente.</summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(long id, CancellationToken ct)
    {
        await eliminarCliente.EliminarAsync(id, ct);
        return NoContent();
    }
}
