using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Application.Service;
using Devsu.Cuentas.Domain.Exceptions;
using Devsu.Cuentas.Domain.Model;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Devsu.Cuentas.UnitTests.Application;

/// <summary>Pruebas del reporte de estado de cuenta (F4).</summary>
public class ReporteEstadoCuentaServiceTests
{
    private readonly IClienteRefRepositoryPort _clienteRefRepo = Substitute.For<IClienteRefRepositoryPort>();
    private readonly ICuentaRepositoryPort _cuentaRepo = Substitute.For<ICuentaRepositoryPort>();
    private readonly IMovimientoRepositoryPort _movimientoRepo = Substitute.For<IMovimientoRepositoryPort>();
    private readonly ReporteEstadoCuentaService _service;

    public ReporteEstadoCuentaServiceTests()
        => _service = new ReporteEstadoCuentaService(_clienteRefRepo, _cuentaRepo, _movimientoRepo);

    [Fact]
    public async Task Genera_una_fila_por_movimiento_con_los_datos_del_cliente_y_la_cuenta()
    {
        var desde = new DateOnly(2022, 2, 1);
        var hasta = new DateOnly(2022, 2, 28);

        _clienteRefRepo.BuscarPorClienteIdAsync("mmontalvo", Arg.Any<CancellationToken>())
            .Returns(new ClienteRef { ClienteId = "mmontalvo", Nombre = "Marianela Montalvo", Estado = true });

        var cuenta = new Cuenta
        {
            Id = 7, NumeroCuenta = "225487", TipoCuenta = TipoCuenta.CORRIENTE,
            SaldoInicial = 100m, SaldoDisponible = 700m, Estado = true, ClienteId = "mmontalvo"
        };
        _cuentaRepo.ListarPorClienteIdAsync("mmontalvo", Arg.Any<CancellationToken>())
            .Returns(new List<Cuenta> { cuenta });

        _movimientoRepo.BuscarPorCuentaIdYRangoAsync(7, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<Movimiento>
            {
                new() { Id = 1, Fecha = new DateTime(2022, 2, 10), TipoMovimiento = TipoMovimiento.DEPOSITO,
                        Valor = 600m, Saldo = 700m, CuentaId = 7 }
            });

        var reporte = await _service.GenerarAsync(desde, hasta, "mmontalvo");

        reporte.Should().HaveCount(1);
        var fila = reporte[0];
        fila.Cliente.Should().Be("Marianela Montalvo");
        fila.NumeroCuenta.Should().Be("225487");
        fila.SaldoInicial.Should().Be(100m);
        fila.Movimiento.Should().Be(600m);
        fila.SaldoDisponible.Should().Be(700m);   // saldo resultante del movimiento
    }

    [Fact]
    public async Task Lanza_404_si_el_cliente_no_existe_en_la_replica()
    {
        _clienteRefRepo.BuscarPorClienteIdAsync("nadie", Arg.Any<CancellationToken>()).Returns((ClienteRef?)null);

        var act = () => _service.GenerarAsync(new DateOnly(2022, 2, 1), new DateOnly(2022, 2, 28), "nadie");

        await act.Should().ThrowAsync<ClienteNoEncontradoException>();
    }
}
