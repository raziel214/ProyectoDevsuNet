using Devsu.Clientes.Domain.Model;
using FluentAssertions;
using Xunit;

namespace Devsu.Clientes.UnitTests.Domain;

/// <summary>
/// Pruebas unitarias de la entidad de dominio Cliente (F5).
/// </summary>
public class ClienteTests
{
    [Fact]
    public void Cliente_se_construye_con_datos_de_persona_y_propios()
    {
        var cliente = new Cliente
        {
            Id = 1,
            Nombre = "Jose Lema",
            Genero = Genero.MASCULINO,
            Edad = 35,
            Identificacion = "0102030405",
            Direccion = "Otavalo sn y principal",
            Telefono = "098254785",
            ClienteId = "jlema",
            Contrasena = "hash",
            Estado = true
        };

        // Hereda los datos personales de Persona...
        cliente.Should().BeAssignableTo<Persona>();
        cliente.Nombre.Should().Be("Jose Lema");
        cliente.Genero.Should().Be(Genero.MASCULINO);
        cliente.Identificacion.Should().Be("0102030405");

        // ...y expone los propios de Cliente.
        cliente.ClienteId.Should().Be("jlema");
        cliente.Contrasena.Should().Be("hash");
        cliente.Estado.Should().BeTrue();
    }

    [Fact]
    public void Dos_clientes_con_mismos_datos_no_son_iguales_por_valor()
    {
        // El agregado se identifica por su instancia (Id), no por todos sus
        // campos: por eso es una clase y NO un record (igualdad por referencia).
        var a = NuevoCliente();
        var b = NuevoCliente();

        a.Should().NotBeSameAs(b);
        a.Equals(b).Should().BeFalse();
    }

    private static Cliente NuevoCliente() => new()
    {
        Id = 1,
        Nombre = "Jose Lema",
        Identificacion = "0102030405",
        ClienteId = "jlema",
        Contrasena = "hash",
        Estado = true
    };
}
