using Devsu.Cuentas.Application.Port.Out;
using Devsu.Cuentas.Application.Service;
using Devsu.Cuentas.Domain.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Devsu.Cuentas.UnitTests.Application;

/// <summary>Pruebas de la sincronización de la réplica de clientes.</summary>
public class ClienteSyncServiceTests
{
    private readonly IClienteRefRepositoryPort _repo = Substitute.For<IClienteRefRepositoryPort>();
    private readonly ILogger<ClienteSyncService> _logger = Substitute.For<ILogger<ClienteSyncService>>();
    private readonly ClienteSyncService _service;

    public ClienteSyncServiceTests() => _service = new ClienteSyncService(_repo, _logger);

    [Fact]
    public async Task Sincronizar_hace_upsert_de_la_replica()
    {
        var cliente = new ClienteRef { ClienteId = "jlema", Nombre = "Jose", Estado = true };

        await _service.SincronizarAsync(cliente);

        await _repo.Received(1).GuardarAsync(cliente, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Desactivar_inhabilita_la_replica_existente_conservando_el_nombre()
    {
        _repo.BuscarPorClienteIdAsync("jlema", Arg.Any<CancellationToken>())
            .Returns(new ClienteRef { ClienteId = "jlema", Nombre = "Jose", Identificacion = "010", Estado = true });
        ClienteRef? guardado = null;
        await _repo.GuardarAsync(Arg.Do<ClienteRef>(c => guardado = c), Arg.Any<CancellationToken>());

        await _service.DesactivarAsync("jlema");

        guardado!.Estado.Should().BeFalse();
        guardado.Nombre.Should().Be("Jose");        // se conserva para reportes historicos
    }

    [Fact]
    public async Task Desactivar_es_idempotente_si_la_replica_no_existe()
    {
        _repo.BuscarPorClienteIdAsync("nadie", Arg.Any<CancellationToken>()).Returns((ClienteRef?)null);

        await _service.DesactivarAsync("nadie");

        await _repo.DidNotReceive().GuardarAsync(Arg.Any<ClienteRef>(), Arg.Any<CancellationToken>());
    }
}
