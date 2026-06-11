using Devsu.Cuentas.Api.Web.Dto;
using Devsu.Cuentas.Api.Web.Mapper;
using Devsu.Cuentas.Application.Port.In;
using Devsu.Cuentas.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.Cuentas.Api.Web.Controllers;

/// <summary>Adaptador de entrada (REST) de Movimientos (F1/F2/F3).</summary>
[ApiController]
[Route("api/movimientos")]
[Produces("application/json")]
public sealed class MovimientosController(
    IRegistrarMovimientoUseCase registrarMovimiento,
    IConsultarMovimientoUseCase consultarMovimiento,
    IActualizarMovimientoUseCase actualizarMovimiento,
    IConsultarCuentaUseCase consultarCuenta) : ControllerBase
{
    /// <summary>Registrar un movimiento (F2/F3). 422 si el retiro supera el saldo.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(MovimientoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<MovimientoResponse>> Registrar([FromBody] MovimientoRequest request, CancellationToken ct)
    {
        var movimiento = await registrarMovimiento.RegistrarAsync(MovimientoWebMapper.ToRegistrarCommand(request), ct);
        // El numeroCuenta ya lo conocemos por la petición.
        var response = MovimientoWebMapper.ToResponse(movimiento, request.NumeroCuenta);
        return CreatedAtAction(nameof(Obtener), new { id = response.Id }, response);
    }

    /// <summary>Listar movimientos (opcionalmente por número de cuenta).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MovimientoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MovimientoResponse>>> Listar([FromQuery] string? numeroCuenta, CancellationToken ct)
    {
        var movimientos = await consultarMovimiento.ListarAsync(numeroCuenta, ct);

        // Caso filtrado: todos los movimientos son de esa cuenta -> no hace falta
        // cargar todas las cuentas, el numeroCuenta es el del filtro.
        if (!string.IsNullOrWhiteSpace(numeroCuenta))
            return Ok(movimientos.Select(m => MovimientoWebMapper.ToResponse(m, numeroCuenta)));

        // Sin filtro: un mapa cuentaId -> numeroCuenta para resolver sin N+1.
        var cuentas = await consultarCuenta.ListarAsync(ct: ct);
        var mapa = cuentas.Where(c => c.Id is not null).ToDictionary(c => c.Id!.Value, c => c.NumeroCuenta);

        return Ok(movimientos.Select(m => MovimientoWebMapper.ToResponse(m, mapa.GetValueOrDefault(m.CuentaId, string.Empty))));
    }

    /// <summary>Obtener movimiento por id.</summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(MovimientoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovimientoResponse>> Obtener(long id, CancellationToken ct)
    {
        var movimiento = await consultarMovimiento.BuscarPorIdAsync(id, ct);
        return Ok(await AResponseAsync(movimiento, ct));
    }

    /// <summary>Editar la descripción (única parte mutable del ledger).</summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(MovimientoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovimientoResponse>> Actualizar(long id, [FromBody] ActualizarMovimientoRequest request, CancellationToken ct)
    {
        var movimiento = await actualizarMovimiento.ActualizarDescripcionAsync(id, request.Descripcion, ct);
        return Ok(await AResponseAsync(movimiento, ct));
    }

    private async Task<MovimientoResponse> AResponseAsync(Movimiento m, CancellationToken ct)
    {
        var cuenta = await consultarCuenta.BuscarPorIdAsync(m.CuentaId, ct);
        return MovimientoWebMapper.ToResponse(m, cuenta.NumeroCuenta);
    }
}
