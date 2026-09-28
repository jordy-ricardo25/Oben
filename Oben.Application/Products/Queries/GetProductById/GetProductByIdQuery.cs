using MediatR;
using Oben.Application.Products.Dtos;

namespace Oben.Application.Products.Queries.GetProductById;

/// <summary>
/// Solicita un producto puntual por identificador.
/// </summary>
public sealed record GetProductByIdQuery(int Id) : IRequest<ProductDto>;

