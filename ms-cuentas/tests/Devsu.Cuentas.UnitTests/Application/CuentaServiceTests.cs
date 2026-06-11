using Devsu.Cuentas.Application.Port.In;
using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Application.Service;
using Devsu.Cuentas.Domain.Exceptions;
using Devsu.Cuentas.Domain.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Devsu.Cuentas.UnitTests.Application;

/// <summary>Pruebas del servicio de Cuentas (F1).</summary>
public class CuentaServiceTests
{
    private readonly ICuentaRepositoryPort _cuentaRepo = Substitute.For<ICuentaRepositoryPort>();
    private readonly IClienteRefRepositoryPort _clienteRefRepo = Substitute.For<IClienteRefRepositoryPort>();
    private readonly ILogger<CuentaService> _logger = Substitute.For<ILogger<CuentaService>>();
    private readonly CuentaService _service;

    public CuentaServiceTests()
    {
        _service = new CuentaService(_cuentaRepo, _clienteRefRepo, _logger);
        _cuentaRepo.GuardarAsync(Arg.Any<Cuenta>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cuenta>());
    }

    [Fact]
    public async Task Crear_valida_cliente_y_arranca_saldo_disponible_igual_al_inicial()
    {
        _clienteRefRepo.BuscarPorClienteIdAsync("jlema", Arg.Any<CancellationToken>())
            .Returns(new ClienteRef { ClienteId = "jlema", Nombre = "Jose", Estado = true });
        _cuentaRepo.ExistePorNumeroCuentaAsync("478758", Arg.Any<CancellationToken>()).Returns(false);
        Cuenta? guardada = null;
        _cuentaRepo.GuardarAsync(Arg.Do<Cuenta>(c => guardada = c), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cuenta>());

        await _service.CrearAsync(new CrearCuentaCommand("478758", TipoCuenta.AHORRO, 2000m, true, "jlema"));

        guardada!.SaldoInicial.Should().Be(2000m);
        guardada.SaldoDisponible.Should().Be(2000m);   // disponible = inicial al crear
    }

    [Fact]
    public async Task Crear_lanza_404_si_el_cliente_no_existe_en_la_replica()
    {
        _clienteRefRepo.BuscarPorClienteIdAsync("nadie", Arg.Any<CancellationToken>()).Returns((ClienteRef?)null);

        var act = () => _service.CrearAsync(new CrearCuentaCommand("478758", TipoCuenta.AHORRO, 2000m, true, "nadie"));

        await act.Should().ThrowAsync<ClienteNoEncontradoException>();
        await _cuentaRepo.DidNotReceive().GuardarAsync(Arg.Any<Cuenta>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Crear_lanza_409_si_el_numero_de_cuenta_ya_existe()
    {
        _clienteRefRepo.BuscarPorClienteIdAsync("jlema", Arg.Any<CancellationToken>())
            .Returns(new ClienteRef { ClienteId = "jlema", Nombre = "Jose", Estado = true });
        _cuentaRepo.ExistePorNumeroCuentaAsync("478758", Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _service.CrearAsync(new CrearCuentaCommand("478758", TipoCuenta.AHORRO, 2000m, true, "jlema"));

        await act.Should().ThrowAsync<NumeroCuentaDuplicadoException>();
    }

    [Fact]
    public async Task CambiarEstado_actualiza_el_estado()
    {
        _cuentaRepo.BuscarPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(Cuenta());
        Cuenta? guardada = null;
        _cuentaRepo.GuardarAsync(Arg.Do<Cuenta>(c => guardada = c), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cuenta>());

        await _service.CambiarEstadoAsync(1, false);

        guardada!.Estado.Should().BeFalse();
    }

    [Fact]
    public async Task Actualizar_cambia_el_tipo()
    {
        _cuentaRepo.BuscarPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(Cuenta());
        Cuenta? guardada = null;
        _cuentaRepo.GuardarAsync(Arg.Do<Cuenta>(c => guardada = c), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cuenta>());

        await _service.ActualizarAsync(1, new ActualizarCuentaCommand(TipoCuenta.CORRIENTE));

        guardada!.TipoCuenta.Should().Be(TipoCuenta.CORRIENTE);
    }

    [Fact]
    public async Task BuscarPorId_lanza_404_si_no_existe()
    {
        _cuentaRepo.BuscarPorIdAsync(99, Arg.Any<CancellationToken>()).Returns((Cuenta?)null);

        var act = () => _service.BuscarPorIdAsync(99);

        await act.Should().ThrowAsync<CuentaNoEncontradaException>();
    }

    private static Cuenta Cuenta() => new()
    {
        Id = 1,
        NumeroCuenta = "478758",
        TipoCuenta = TipoCuenta.AHORRO,
        SaldoInicial = 2000m,
        SaldoDisponible = 2000m,
        Estado = true,
        ClienteId = "jlema"
    };
}
