using MediatR;
using Oben.Application.Products.Dtos;

namespace Oben.Application.Products.Queries.GetProducts;

/// <summary>
/// Solicita el listado completo de productos requerido por el CRUD de la prueba.
/// </summary>
public sealed record GetProductsQuery : IRequest<IReadOnlyCollection<ProductDto>>;

