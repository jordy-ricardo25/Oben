using MediatR;
using Microsoft.AspNetCore.Mvc;
using Oben.Api.Models.Auth;
using Oben.Application.Auth.Commands.Login;
using Oben.Application.Auth.Dtos;

namespace Oben.Api.Controllers;

/// <summary>
/// Endpoints de autenticacion de la prueba tecnica.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender sender;

    /// <summary>
    /// Recibe MediatR como punto unico de entrada a los casos de uso.
    /// </summary>
    public AuthController(ISender sender)
    {
        this.sender = sender;
    }

    /// <summary>
    /// Autentica un usuario existente y devuelve una sesion minima.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        return Ok(response);
    }
}
