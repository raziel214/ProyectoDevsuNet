using Devsu.Clientes.Application.Port.In;
using Devsu.Clientes.Application.Port.Out;
using Devsu.Clientes.Application.Service;
using Devsu.Clientes.Domain.Exceptions;
using Devsu.Clientes.Domain.Model;
using FluentAssertions;
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
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _service = new ClienteService(_repo, _hasher, _publisher);
        // Por defecto el repositorio devuelve el cliente que recibe al guardar.
        _repo.GuardarAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cliente>());
    }

    // ---------- Crear ----------

    [Fact]
    public async Task Crear_hashea_la_contrasena_guarda_y_publica_evento()
    {
        _hasher.Hash("1234").Returns("HASH");
        Cliente? guardado = null;
        _repo.GuardarAsync(Arg.Do<Cliente>(c => guardado = c), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cliente>());

        var result = await _service.CrearAsync(Comando());

        guardado!.Contrasena.Should().Be("HASH");          // se persiste el hash, no el texto plano
        guardado.Estado.Should().BeTrue();
        await _publisher.Received(1).PublicarCreadoAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
        result.ClienteId.Should().Be("jlema");
    }

    [Fact]
    public async Task Crear_asigna_estado_true_cuando_el_comando_no_lo_trae()
    {
        Cliente? guardado = null;
        _repo.GuardarAsync(Arg.Do<Cliente>(c => guardado = c), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cliente>());

        await _service.CrearAsync(Comando() with { Estado = null });

        guardado!.Estado.Should().BeTrue();
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

    // ---------- Actualizar ----------

    [Fact]
    public async Task Actualizar_conserva_la_identidad_y_publica_evento()
    {
        _repo.BuscarPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(ClienteExistente());
        Cliente? guardado = null;
        _repo.GuardarAsync(Arg.Do<Cliente>(c => guardado = c), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cliente>());

        await _service.ActualizarAsync(1, ComandoActualizar());

        // La identidad (Id, ClienteId, Identificacion) NO cambia.
        guardado!.Id.Should().Be(1);
        guardado.ClienteId.Should().Be("jlema");
        guardado.Identificacion.Should().Be("0102030405");
        // Los datos modificables sí.
        guardado.Nombre.Should().Be("Jose Actualizado");
        await _publisher.Received(1).PublicarActualizadoAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Actualizar_conserva_la_contrasena_si_el_comando_no_la_trae()
    {
        _repo.BuscarPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(ClienteExistente());
        Cliente? guardado = null;
        _repo.GuardarAsync(Arg.Do<Cliente>(c => guardado = c), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cliente>());

        await _service.ActualizarAsync(1, ComandoActualizar() with { Contrasena = null });

        guardado!.Contrasena.Should().Be("HASH_EXISTENTE");        // conserva la actual
        _hasher.DidNotReceive().Hash(Arg.Any<string>());          // no re-hashea
    }

    [Fact]
    public async Task Actualizar_rehashea_si_llega_una_contrasena_nueva()
    {
        _repo.BuscarPorIdAsync(1, Arg.Any<CancellationToken>()).Returns(ClienteExistente());
        _hasher.Hash("nueva").Returns("HASH_NUEVO");
        Cliente? guardado = null;
        _repo.GuardarAsync(Arg.Do<Cliente>(c => guardado = c), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cliente>());

        await _service.ActualizarAsync(1, ComandoActualizar() with { Contrasena = "nueva" });

        guardado!.Contrasena.Should().Be("HASH_NUEVO");
    }

    [Fact]
    public async Task Actualizar_lanza_no_encontrado_cuando_no_existe()
    {
        _repo.BuscarPorIdAsync(99, Arg.Any<CancellationToken>()).Returns((Cliente?)null);

        var act = () => _service.ActualizarAsync(99, ComandoActualizar());

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

    private static CrearClienteCommand Comando() => new(
        Nombre: "Jose Lema", Genero: Genero.MASCULINO, Edad: 35,
        Identificacion: "0102030405", Direccion: "Otavalo", Telefono: "098254785",
        ClienteId: "jlema", Contrasena: "1234", Estado: true);

    private static ActualizarClienteCommand ComandoActualizar() => new(
        Nombre: "Jose Actualizado", Genero: Genero.MASCULINO, Edad: 36,
        Direccion: "Nueva dir", Telefono: "099999999", Contrasena: "1234", Estado: true);

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
