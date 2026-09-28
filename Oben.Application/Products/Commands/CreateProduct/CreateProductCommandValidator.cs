using FluentValidation;

namespace Oben.Application.Products.Commands.CreateProduct;

/// <summary>
/// Valida la entrada del caso de uso de creacion antes de construir la entidad de dominio.
/// </summary>
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    /// <summary>
    /// Mantiene aqui reglas de request; las invariantes finales siguen viviendo en dominio.
    /// </summary>
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty();

        RuleFor(command => command.Price)
            .GreaterThan(0);

        RuleFor(command => command.Stock)
            .GreaterThanOrEqualTo(0);
    }
}

