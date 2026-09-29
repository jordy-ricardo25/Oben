using MediatR;
using Oben.Application.Auth.Contracts;
using Oben.Application.Auth.Dtos;
using Oben.Application.Common.Exceptions;

namespace Oben.Application.Auth.Commands.Login;

/// <summary>
/// Orquesta el caso de uso de login sin asumir como se almacenan usuarios ni como se emiten tokens.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IAuthenticationService authenticationService;

    /// <summary>
    /// Recibe el servicio concreto de autenticacion a traves de su contrato de aplicacion.
    /// </summary>
    public LoginCommandHandler(IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }

    /// <summary>
    /// Autentica al usuario o lanza una excepcion de aplicacion para credenciales invalidas.
    /// </summary>
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var response = await authenticationService.AuthenticateAsync(
            request.Email,
            request.Password,
            cancellationToken);

        return response ?? throw new UnauthorizedException("Invalid email or password.");
    }
}
