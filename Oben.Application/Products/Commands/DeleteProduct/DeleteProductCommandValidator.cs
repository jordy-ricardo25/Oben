using FluentValidation;

namespace Oben.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Valida identificadores necesarios para eliminar un producto y auditar la operacion.
/// </summary>
public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    /// <summary>
    /// Evita llamadas a infraestructura con identificadores imposibles.
    /// </summary>
    public DeleteProductCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);

        RuleFor(command => command.UserId)
            .GreaterThan(0);
    }
}

