namespace Oben.Maui.Services;

/// <summary>
/// Configuracion minima del cliente HTTP usado por la app MAUI.
/// </summary>
public sealed class ApiOptions
{
    public ApiOptions(string baseAddress)
    {
        BaseAddress = new Uri(baseAddress, UriKind.Absolute);
    }

    /// <summary>
    /// URL base de Oben.Api. En Windows con API local se usa localhost.
    /// </summary>
    public Uri BaseAddress { get; }
}
