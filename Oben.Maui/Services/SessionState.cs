using Oben.Maui.Services.Models;

namespace Oben.Maui.Services;

/// <summary>
/// Mantiene la sesion en memoria durante la ejecucion de la app.
/// </summary>
public sealed class SessionState
{
    public LoginResponse? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser is not null;

    public void SignIn(LoginResponse user)
    {
        CurrentUser = user;
    }

    public void SignOut()
    {
        CurrentUser = null;
    }
}
