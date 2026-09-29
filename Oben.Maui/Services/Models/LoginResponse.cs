namespace Oben.Maui.Services.Models;

public sealed record LoginResponse(
    int UserId,
    string Name,
    string Email,
    string AccessToken);
