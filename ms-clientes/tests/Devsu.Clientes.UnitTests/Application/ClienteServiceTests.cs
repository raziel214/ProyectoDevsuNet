using Devsu.Clientes.Application.Port.In;
using Devsu.Clientes.Application.Port.Out;
using Devsu.Clientes.Application.Service;
using Devsu.Clientes.Domain.Exceptions;
using Devsu.Clientes.Domain.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Devsu.Clientes.UnitTests.Application;

/// <summary>
/// Pruebas unitarias del servicio de aplicación (casos de uso del CRUD, F5).
/// Los puertos de salida se sustituyen con dobles (NSubstitute), de modo que se
/// prueba la lógica del servicio aislada de infraestructura.
/// </summary>
public class ClienteServiceTests
{
    private readonly IClienteRepositoryPort _repo = Substitute.For<IClienteRepositoryPort>();
    private readonly IPasswordHasherPort _hasher = Substitute.For<IPasswordHasherPort>();
    private readonly IClienteEventPublisherPort _publisher = Substitute.For<IClienteEventPublisherPort>();
    private readonly ILogger<ClienteService> _logger = Substitute.For<ILogger<ClienteService>>();
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _service = new ClienteService(_repo, _hasher, _publisher, _logger);
        // Por defecto el repositorio devuelve el cliente que recibe al guardar.
        _repo.GuardarAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cliente>());
    }

    // ---------- Crear ----------

    [Fact]
    public async Task Crear_hashea_la_contrasena_guarda_y_publica_evento()
    {
        _hasher.Hash("1234").Returns("HASH");
        var guardado = CapturarGuardado();

        var result = await _service.CrearAsync(Comando());

        guardado().Contrasena.Should().Be("HASH");         // se persiste el hash, no el texto plano
        guardado().Estado.Should().BeTrue();
        await _publisher.Received(1).PublicarCreadoAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
        result.ClienteId.Should().Be("jlema");
    }

    [Fact]
    public async Task Crear_asigna_estado_true_cuando_el_comando_no_lo_trae()
    {
        var guardado = CapturarGuardado();

        await _service.CrearAsync(Comando() with { Estado = null });

        guardado().Estado.Should().BeTrue();
    }

    [Fact]
    public async Task Crear_lanza_duplicado_si_el_clienteId_ya_existe()
    {
        _repo.ExistePorClienteIdAsync("jlema", Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _service.CrearAsync(Comando());

        await act.Should().ThrowAsync<ClienteDuplicadoException>();
        await _repo.DidNotReceive().GuardarAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Crear_lanza_duplicado_si_la_identificacion_ya_existe()
    {
        _repo.ExistePorIdentificacionAsync("0102030405", Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _service.CrearAsync(Comando());

        await act.Should().ThrowAsync<ClienteDuplicadoException>();
    }

    // ---------- Consultar ----------

    [Fact]
    public async Task BuscarPorId_devuelve_el_cliente_cuando_existe()
    {
        var cliente = ClienteExistente();
        _repo.BuscarPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(cliente);

        var result = await _service.BuscarPorIdAsync(1);

        result.Should().BeSameAs(cliente);
    }

    [Fact]
    public async Task BuscarPorId_lanza_no_encontrado_cuando_no_existe()
    {
        _repo.BuscarPorIdAsync(99, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

        var act = () => _service.BuscarPorIdAsync(99);

        await act.Should().ThrowAsync<ClienteNoEncontradoException>();
    }

    // ---------- Actualizar perfil ----------

    [Fact]
    public async Task Actualizar_cambia_perfil_y_conserva_identidad_estado_y_contrasena()
    {
        _repo.BuscarPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(ClienteExistente());
        var guardado = CapturarGuardado();

        await _service.ActualizarAsync(1, PerfilComando());

        // La identidad, estado y credencial NO cambian con el perfil.
        guardado().Id.Should().Be(1);
        guardado().ClienteId.Should().Be("jlema");
        guardado().Identificacion.Should().Be("0102030405");
        guardado().Estado.Should().BeTrue();
        guardado().Contrasena.Should().Be("HASH_EXISTENTE");
        // Los datos de perfil sí.
        guardado().Nombre.Should().Be("Jose Actualizado");
        await _publisher.Received(1).PublicarActualizadoAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Actualizar_lanza_no_encontrado_cuando_no_existe()
    {
        _repo.BuscarPorIdAsync(99, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

        var act = () => _service.ActualizarAsync(99, PerfilComando());

        await act.Should().ThrowAsync<ClienteNoEncontradoException>();
    }

    // ---------- Cambiar estado ----------

    [Fact]
    public async Task CambiarEstado_actualiza_el_estado_y_publica_evento()
    {
        _repo.BuscarPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(ClienteExistente());
        var guardado = CapturarGuardado();

        await _service.CambiarEstadoAsync(1, estado: false);

        guardado().Estado.Should().BeFalse();
        guardado().ClienteId.Should().Be("jlema");          // identidad intacta
        await _publisher.Received(1).PublicarActualizadoAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CambiarEstado_lanza_no_encontrado_cuando_no_existe()
    {
        _repo.BuscarPorIdAsync(99, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

        var act = () => _service.CambiarEstadoAsync(99, estado: false);

        await act.Should().ThrowAsync<ClienteNoEncontradoException>();
    }

    // ---------- Cambiar contraseña ----------

    [Fact]
    public async Task CambiarContrasena_verifica_la_actual_rehashea_la_nueva_y_no_publica()
    {
        _repo.BuscarPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(ClienteExistente());
        _hasher.Verify("actual", "HASH_EXISTENTE").Returns(true);
        _hasher.Hash("nueva").Returns("HASH_NUEVO");
        var guardado = CapturarGuardado();

        await _service.CambiarContrasenaAsync(1, new CambiarContrasenaCommand("actual", "nueva"));

        guardado().Contrasena.Should().Be("HASH_NUEVO");
        // La contraseña no afecta la réplica de cuentas: no se publica evento.
        await _publisher.DidNotReceive().PublicarActualizadoAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CambiarContrasena_lanza_si_la_actual_es_incorrecta()
    {
        _repo.BuscarPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(ClienteExistente());
        _hasher.Verify("mala", "HASH_EXISTENTE").Returns(false);

        var act = () => _service.CambiarContrasenaAsync(1, new CambiarContrasenaCommand("mala", "nueva"));

        await act.Should().ThrowAsync<ContrasenaActualInvalidaException>();
        await _repo.DidNotReceive().GuardarAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CambiarContrasena_lanza_no_encontrado_cuando_no_existe()
    {
        _repo.BuscarPorIdAsync(99, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

        var act = () => _service.CambiarContrasenaAsync(99, new CambiarContrasenaCommand("a", "b"));

        await act.Should().ThrowAsync<ClienteNoEncontradoException>();
    }

    // ---------- Eliminar ----------

    [Fact]
    public async Task Eliminar_borra_y_publica_evento_con_el_clienteId()
    {
        _repo.BuscarPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(ClienteExistente());

        await _service.EliminarAsync(1);

        await _repo.Received(1).EliminarPorIdAsync(1, Arg.Any<CancellationToken>());
        await _publisher.Received(1).PublicarEliminadoAsync("jlema", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Eliminar_lanza_no_encontrado_cuando_no_existe()
    {
        _repo.BuscarPorIdAsync(99, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

        var act = () => _service.EliminarAsync(99);

        await act.Should().ThrowAsync<ClienteNoEncontradoException>();
        await _repo.DidNotReceive().EliminarPorIdAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    // ---------- Helpers ----------

    /// <summary>Captura el Cliente pasado a GuardarAsync para inspeccionarlo.</summary>
    private Func<Cliente> CapturarGuardado()
    {
        Cliente? guardado = null;
        _repo.GuardarAsync(Arg.Do<Cliente>(c => guardado = c), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cliente>());
        return () => guardado!;
    }

    private static CrearClienteCommand Comando() => new(
        Nombre: "Jose Lema", Genero: Genero.MASCULINO, Edad: 35,
        Identificacion: "0102030405", Direccion: "Otavalo", Telefono: "098254785",
        ClienteId: "jlema", Contrasena: "1234", Estado: true);

    private static ActualizarPerfilCommand PerfilComando() => new(
        Nombre: "Jose Actualizado", Genero: Genero.MASCULINO, Edad: 36,
        Direccion: "Nueva dir", Telefono: "099999999");

    private static Cliente ClienteExistente() => new()
    {
        Id = 1,
        Nombre = "Jose Lema",
        Genero = Genero.MASCULINO,
        Edad = 35,
        Identificacion = "0102030405",
        Direccion = "Otavalo",
        Telefono = "098254785",
        ClienteId = "jlema",
        Contrasena = "HASH_EXISTENTE",
        Estado = true
    };
}
