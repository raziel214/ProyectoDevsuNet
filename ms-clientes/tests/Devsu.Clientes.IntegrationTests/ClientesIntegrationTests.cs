using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Devsu.Clientes.IntegrationTests;

/// <summary>
/// Pruebas de integración del CRUD de Cliente (F6) sobre el API real
/// (WebApplicationFactory) con Postgres y RabbitMQ efímeros (Testcontainers).
/// </summary>
public class ClientesIntegrationTests(ClientesApiFactory factory) : IClassFixture<ClientesApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Crear_y_obtener_cliente()
    {
        var crear = await _client.PostAsJsonAsync("/api/clientes", NuevoCliente("a1"));
        crear.StatusCode.Should().Be(HttpStatusCode.Created);

        var creado = await crear.Content.ReadFromJsonAsync<ClienteDto>();
        creado!.Id.Should().BeGreaterThan(0);
        creado.ClienteId.Should().Be("clia1");

        var obtener = await _client.GetAsync($"/api/clientes/{creado.Id}");
        obtener.StatusCode.Should().Be(HttpStatusCode.OK);
        var obtenido = await obtener.Content.ReadFromJsonAsync<ClienteDto>();
        obtenido!.Nombre.Should().Be("Test a1");
    }

    [Fact]
    public async Task Crear_duplicado_devuelve_409()
    {
        await _client.PostAsJsonAsync("/api/clientes", NuevoCliente("dup"));
        var segundo = await _client.PostAsJsonAsync("/api/clientes", NuevoCliente("dup"));

        segundo.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Crear_invalido_devuelve_400()
    {
        var invalido = new { genero = "MASCULINO", edad = 30, estado = true }; // sin nombre/identificacion/clienteId/contrasena
        var resp = await _client.PostAsJsonAsync("/api/clientes", invalido);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Obtener_inexistente_devuelve_404()
    {
        var resp = await _client.GetAsync("/api/clientes/999999");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Actualizar_perfil_cambia_el_nombre()
    {
        var creado = await CrearAsync("upd");

        var perfil = new { nombre = "Nombre Nuevo", genero = "FEMENINO", edad = 41, direccion = "x", telefono = "1" };
        var resp = await _client.PutAsJsonAsync($"/api/clientes/{creado.Id}", perfil);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var actualizado = await resp.Content.ReadFromJsonAsync<ClienteDto>();
        actualizado!.Nombre.Should().Be("Nombre Nuevo");
    }

    [Fact]
    public async Task Cambiar_estado_inhabilita_al_cliente()
    {
        var creado = await CrearAsync("est");

        var resp = await _client.PatchAsJsonAsync($"/api/clientes/{creado.Id}/estado", new { estado = false });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var actualizado = await resp.Content.ReadFromJsonAsync<ClienteDto>();
        actualizado!.Estado.Should().BeFalse();
    }

    [Fact]
    public async Task Cambiar_contrasena_verifica_la_actual()
    {
        var creado = await CrearAsync("pwd");

        var ok = await _client.PostAsJsonAsync($"/api/clientes/{creado.Id}/contrasena",
            new { contrasenaActual = "1234", contrasenaNueva = "5678" });
        ok.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var mal = await _client.PostAsJsonAsync($"/api/clientes/{creado.Id}/contrasena",
            new { contrasenaActual = "incorrecta", contrasenaNueva = "9999" });
        mal.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Eliminar_cliente_y_luego_404()
    {
        var creado = await CrearAsync("del");

        var del = await _client.DeleteAsync($"/api/clientes/{creado.Id}");
        del.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var obtener = await _client.GetAsync($"/api/clientes/{creado.Id}");
        obtener.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<ClienteDto> CrearAsync(string sufijo)
    {
        var resp = await _client.PostAsJsonAsync("/api/clientes", NuevoCliente(sufijo));
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<ClienteDto>())!;
    }

    private static object NuevoCliente(string sufijo, string contrasena = "1234") => new
    {
        nombre = "Test " + sufijo,
        genero = "MASCULINO",
        edad = 30,
        identificacion = "ID" + sufijo,
        direccion = "dir",
        telefono = "099",
        clienteId = "cli" + sufijo,
        contrasena,
        estado = true
    };

    private sealed record ClienteDto(long Id, string Nombre, string ClienteId, bool Estado);
}
