namespace Devsu.Clientes.Application.Port.In;

/// <summary>
/// Datos de entrada para cambiar la contraseña de un cliente (caso de uso
/// <see cref="ICambiarContrasenaClienteUseCase"/>).
///
/// <para>Operación sensible: exige la <c>ContrasenaActual</c> (se verifica contra
/// la almacenada) además de la <c>ContrasenaNueva</c>.</para>
/// </summary>
public record CambiarContrasenaCommand(
    string ContrasenaActual,
    string ContrasenaNueva
);
