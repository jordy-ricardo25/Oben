using Oben.Application.Auth.Dtos;

namespace Oben.Application.Auth.Contracts;

/// <summary>
/// Puerto de autenticacion usado por la capa de aplicacion.
/// La validacion real de credenciales, hashing y token pertenecen a infraestructura.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Devuelve la sesion autenticada cuando las credenciales son validas; en caso contrario devuelve null.
    /// </summary>
    Task<LoginResponse?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken);
}
