namespace Oben.Application.Auth.Dtos;

/// <summary>
/// Resultado minimo que la aplicacion necesita despues de autenticar a un usuario.
/// </summary>
public sealed record LoginResponse(
    int UserId,
    string Name,
    string Email,
    string AccessToken);
