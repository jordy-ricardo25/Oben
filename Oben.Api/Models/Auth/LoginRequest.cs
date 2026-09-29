namespace Oben.Api.Models.Auth;

/// <summary>
/// Entrada HTTP para iniciar sesion.
/// </summary>
public sealed record LoginRequest(string Email, string Password);
