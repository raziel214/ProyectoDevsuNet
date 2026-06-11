using Devsu.Cuentas.Api.Web.Dto;
using Devsu.Cuentas.Api.Web.Mapper;
using Devsu.Cuentas.Application.Port.In;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.Cuentas.Api.Web.Controllers;

/// <summary>Adaptador de entrada (REST) del CRU de Cuentas (F1).</summary>
[ApiController]
[Route("api/cuentas")]
[Produces("application/json")]
public sealed class CuentasController(
    ICrearCuentaUseCase crearCuenta,
    IConsultarCuentaUseCase consultarCuenta,
    IActualizarCuentaUseCase actualizarCuenta,
    ICambiarEstadoCuentaUseCase cambiarEstadoCuenta) : ControllerBase
{
    /// <summary>Crear cuenta.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CuentaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CuentaResponse>> Crear([FromBody] CuentaRequest request, CancellationToken ct)
    {
        var cuenta = await crearCuenta.CrearAsync(CuentaWebMapper.ToCrearCommand(request), ct);
        var response = CuentaWebMapper.ToResponse(cuenta);
        return CreatedAtAction(nameof(Obtener), new { id = response.Id }, response);
    }

    /// <summary>Listar cuentas (opcionalmente filtradas por cliente).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CuentaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CuentaResponse>>> Listar([FromQuery] string? clienteId, CancellationToken ct)
    {
        var cuentas = await consultarCuenta.ListarAsync(clienteId, ct);
        return Ok(cuentas.Select(CuentaWebMapper.ToResponse));
    }

    /// <summary>Obtener cuenta por id.</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CuentaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CuentaResponse>> Obtener(long id, CancellationToken ct)
    {
        var cuenta = await consultarCuenta.BuscarPorIdAsync(id, ct);
        return Ok(CuentaWebMapper.ToResponse(cuenta));
    }

    /// <summary>Actualizar los datos editables de una cuenta (tipo).</summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(CuentaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CuentaResponse>> Actualizar(long id, [FromBody] ActualizarCuentaRequest request, CancellationToken ct)
    {
        var cuenta = await actualizarCuenta.ActualizarAsync(id, CuentaWebMapper.ToActualizarCommand(request), ct);
        return Ok(CuentaWebMapper.ToResponse(cuenta));
    }

    /// <summary>Habilitar/inhabilitar una cuenta (operación sensible, auditada).</summary>
    [HttpPatch("{id:long}/estado")]
    [ProducesResponseType(typeof(CuentaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CuentaResponse>> CambiarEstado(long id, [FromBody] CambiarEstadoCuentaRequest request, CancellationToken ct)
    {
        var cuenta = await cambiarEstadoCuenta.CambiarEstadoAsync(id, request.Estado!.Value, ct);
        return Ok(CuentaWebMapper.ToResponse(cuenta));
    }
}
