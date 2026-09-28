using FluentValidation;

namespace Oben.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Valida la forma de la solicitud de actualizacion, incluido el usuario que alimenta la auditoria.
/// </summary>
public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    /// <summary>
    /// Rechaza identificadores invalidos y valores que no pueden llegar al dominio.
    /// </summary>
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);

        RuleFor(command => command.Name)
            .NotEmpty();

        RuleFor(command => command.Price)
            .GreaterThan(0);

        RuleFor(command => command.Stock)
            .GreaterThanOrEqualTo(0);

        RuleFor(command => command.UserId)
            .GreaterThan(0);
    }
}

