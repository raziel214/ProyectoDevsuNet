using Devsu.Cuentas.Application.Port.In;
using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Application.Service;
using Devsu.Cuentas.Domain.Exceptions;
using Devsu.Cuentas.Domain.Model;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Devsu.Cuentas.UnitTests.Application;

/// <summary>Pruebas del servicio de Movimientos (F2/F3).</summary>
public class MovimientoServiceTests
{
    private readonly ICuentaRepositoryPort _cuentaRepo = Substitute.For<ICuentaRepositoryPort>();
    private readonly IMovimientoRepositoryPort _movimientoRepo = Substitute.For<IMovimientoRepositoryPort>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly MovimientoService _service;

    public MovimientoServiceTests()
    {
        _service = new MovimientoService(_cuentaRepo, _movimientoRepo, _uow);

        // El UnitOfWork ejecuta la accion recibida (simula la transaccion).
        _uow.ExecuteInTransactionAsync(Arg.Any<Func<CancellationToken, Task>>(), Arg.Any<CancellationToken>())
            .Returns(call => ((Func<CancellationToken, Task>)call[0])(CancellationToken.None));

        // El repo de movimientos devuelve el que recibe (con su saldo/tipo).
        _movimientoRepo.GuardarAsync(Arg.Any<Movimiento>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Movimiento>());
    }

    [Fact]
    public async Task Registrar_deposito_actualiza_saldo_y_crea_asiento()
    {
        _cuentaRepo.BuscarPorNumeroCuentaAsync("478758", Arg.Any<CancellationToken>())
            .Returns(Cuenta(saldo: 1000m));
        Cuenta? cuentaGuardada = null;
        _cuentaRepo.GuardarAsync(Arg.Do<Cuenta>(c => cuentaGuardada = c), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cuenta>());

        var mov = await _service.RegistrarAsync(new RegistrarMovimientoCommand("478758", 600m));

        mov.TipoMovimiento.Should().Be(TipoMovimiento.DEPOSITO);
        mov.Saldo.Should().Be(1600m);            // saldo resultante
        cuentaGuardada!.SaldoDisponible.Should().Be(1600m);
        await _movimientoRepo.Received(1).GuardarAsync(Arg.Any<Movimiento>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Registrar_retiro_valido_disminuye_el_saldo()
    {
        _cuentaRepo.BuscarPorNumeroCuentaAsync("478758", Arg.Any<CancellationToken>())
            .Returns(Cuenta(saldo: 1000m));

        var mov = await _service.RegistrarAsync(new RegistrarMovimientoCommand("478758", -575m));

        mov.TipoMovimiento.Should().Be(TipoMovimiento.RETIRO);
        mov.Saldo.Should().Be(425m);
    }

    [Fact]
    public async Task Registrar_retiro_sin_fondos_lanza_422_y_no_persiste()
    {
        _cuentaRepo.BuscarPorNumeroCuentaAsync("478758", Arg.Any<CancellationToken>())
            .Returns(Cuenta(saldo: 100m));

        var act = () => _service.RegistrarAsync(new RegistrarMovimientoCommand("478758", -200m));

        await act.Should().ThrowAsync<SaldoNoDisponibleException>();
        await _movimientoRepo.DidNotReceive().GuardarAsync(Arg.Any<Movimiento>(), Arg.Any<CancellationToken>());
        await _cuentaRepo.DidNotReceive().GuardarAsync(Arg.Any<Cuenta>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Registrar_sobre_cuenta_inexistente_lanza_404()
    {
        _cuentaRepo.BuscarPorNumeroCuentaAsync("000", Arg.Any<CancellationToken>()).Returns((Cuenta?)null);

        var act = () => _service.RegistrarAsync(new RegistrarMovimientoCommand("000", 100m));

        await act.Should().ThrowAsync<CuentaNoEncontradaException>();
    }

    [Fact]
    public async Task ActualizarDescripcion_edita_solo_la_descripcion()
    {
        var existente = new Movimiento
        {
            Id = 5, Fecha = new DateTime(2022, 2, 10), TipoMovimiento = TipoMovimiento.DEPOSITO,
            Valor = 600m, Saldo = 1600m, CuentaId = 1, Descripcion = null
        };
        _movimientoRepo.BuscarPorIdAsync(5, Arg.Any<CancellationToken>()).Returns(existente);
        Movimiento? guardado = null;
        _movimientoRepo.GuardarAsync(Arg.Do<Movimiento>(m => guardado = m), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Movimiento>());

        await _service.ActualizarDescripcionAsync(5, "ajuste manual");

        guardado!.Descripcion.Should().Be("ajuste manual");
        guardado.Valor.Should().Be(600m);        // el ledger no cambia
        guardado.Saldo.Should().Be(1600m);
    }

    [Fact]
    public async Task ActualizarDescripcion_lanza_404_si_no_existe()
    {
        _movimientoRepo.BuscarPorIdAsync(99, Arg.Any<CancellationToken>()).Returns((Movimiento?)null);

        var act = () => _service.ActualizarDescripcionAsync(99, "x");

        await act.Should().ThrowAsync<MovimientoNoEncontradoException>();
    }

    private static Cuenta Cuenta(decimal saldo) => new()
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
