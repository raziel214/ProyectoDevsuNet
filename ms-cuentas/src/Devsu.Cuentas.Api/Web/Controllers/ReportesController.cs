using Devsu.Cuentas.Api.Web.Dto;
using Devsu.Cuentas.Api.Web.Mapper;
using Devsu.Cuentas.Application.Port.In;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.Cuentas.Api.Web.Controllers;

/// <summary>Adaptador de entrada (REST) del reporte "Estado de cuenta" (F4).</summary>
[ApiController]
[Route("api/reportes")]
[Produces("application/json")]
public sealed class ReportesController(IReporteEstadoCuentaUseCase reporte) : ControllerBase
{
    /// <summary>Estado de cuenta por rango de fechas y cliente (F4).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReporteEstadoCuentaRow>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ReporteEstadoCuentaRow>>> EstadoDeCuenta(
        [FromQuery] DateOnly fechaInicio,
        [FromQuery] DateOnly fechaFin,
        [FromQuery] string cliente,
        CancellationToken ct)
    {
        var items = await reporte.GenerarAsync(fechaInicio, fechaFin, cliente, ct);
        return Ok(items.Select(ReporteWebMapper.ToRow));
    }
}
