using Devsu.Cuentas.Domain.Exceptions;
using Devsu.Cuentas.Domain.Model;
using FluentAssertions;
using Xunit;

namespace Devsu.Cuentas.UnitTests.Domain;

/// <summary>
/// Pruebas unitarias del dominio de Cuenta (F5). La invariante clave es la
/// validación de saldo al aplicar un movimiento (F3).
/// </summary>
public class CuentaTests
{
    [Fact]
    public void AplicarMovimiento_deposito_aumenta_el_saldo()
    {
        var cuenta = NuevaCuenta(saldo: 1000m);

        var resultado = cuenta.AplicarMovimiento(600m);

        resultado.SaldoDisponible.Should().Be(1600m);
    }

    [Fact]
    public void AplicarMovimiento_retiro_disminuye_el_saldo()
    {
        var cuenta = NuevaCuenta(saldo: 1000m);

        var resultado = cuenta.AplicarMovimiento(-575m);

        resultado.SaldoDisponible.Should().Be(425m);
    }

    [Fact]
    public void AplicarMovimiento_retiro_exacto_deja_saldo_en_cero()
    {
        var cuenta = NuevaCuenta(saldo: 500m);

        var resultado = cuenta.AplicarMovimiento(-500m);

        resultado.SaldoDisponible.Should().Be(0m);
    }

    [Fact]
    public void AplicarMovimiento_retiro_sin_fondos_lanza_saldo_no_disponible()
    {
        var cuenta = NuevaCuenta(saldo: 100m);

        var act = () => cuenta.AplicarMovimiento(-200m);

        act.Should().Throw<SaldoNoDisponibleException>()
            .WithMessage("Saldo no disponible");
    }

    [Fact]
    public void CambiarEstado_y_CambiarTipo_conservan_la_identidad()
    {
        var cuenta = NuevaCuenta(saldo: 100m);

        var inhabilitada = cuenta.CambiarEstado(false);
        var corriente = cuenta.CambiarTipo(TipoCuenta.CORRIENTE);

        inhabilitada.Estado.Should().BeFalse();
        inhabilitada.NumeroCuenta.Should().Be("478758");   // identidad intacta
        corriente.TipoCuenta.Should().Be(TipoCuenta.CORRIENTE);
        corriente.SaldoDisponible.Should().Be(100m);
    }

    private static Cuenta NuevaCuenta(decimal saldo) => new()
    {
        Id = 1,
        NumeroCuenta = "478758",
        TipoCuenta = TipoCuenta.AHORRO,
        SaldoInicial = saldo,
        SaldoDisponible = saldo,
        Estado = true,
        ClienteId = "jlema"
    };
}
