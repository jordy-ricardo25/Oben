using FluentValidation;

namespace Oben.Application.Auth.Commands.Login;

/// <summary>
/// Valida la forma de la solicitud de login antes de ejecutar autenticacion.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Define reglas de entrada, no reglas de seguridad ni persistencia.
    /// </summary>
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(command => command.Password)
            .NotEmpty();
    }
}
