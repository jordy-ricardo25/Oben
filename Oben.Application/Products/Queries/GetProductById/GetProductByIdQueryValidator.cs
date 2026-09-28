using FluentValidation;

namespace Oben.Application.Products.Queries.GetProductById;

/// <summary>
/// Valida la forma minima de una consulta por identificador.
/// </summary>
public sealed class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    /// <summary>
    /// Evita buscar productos con identificadores imposibles.
    /// </summary>
    public GetProductByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0);
    }
}

