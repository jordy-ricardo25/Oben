using MediatR;
using Oben.Application.Auth.Dtos;

namespace Oben.Application.Auth.Commands.Login;

/// <summary>
/// Solicita autenticacion con correo y contrasena.
/// </summary>
public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
