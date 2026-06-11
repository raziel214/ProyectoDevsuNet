using System.Text.Json.Serialization;

namespace Devsu.Cuentas.Api.Web.Dto;

/// <summary>
/// Fila del reporte de estado de cuenta (F4). Las claves JSON respetan
/// exactamente el formato del enunciado (capitalizadas y con espacios).
/// </summary>
public record ReporteEstadoCuentaRow
{
    [JsonPropertyName("Fecha")]
    public string Fecha { get; init; } = null!;

    [JsonPropertyName("Cliente")]
    public string Cliente { get; init; } = null!;

    [JsonPropertyName("Numero Cuenta")]
    public string NumeroCuenta { get; init; } = null!;

    [JsonPropertyName("Tipo")]
    public string Tipo { get; init; } = null!;

    [JsonPropertyName("Saldo Inicial")]
    public decimal SaldoInicial { get; init; }

    [JsonPropertyName("Estado")]
    public bool Estado { get; init; }

    [JsonPropertyName("Movimiento")]
    public decimal Movimiento { get; init; }

    [JsonPropertyName("Saldo Disponible")]
    public decimal SaldoDisponible { get; init; }
}
