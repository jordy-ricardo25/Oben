using MediatR;
using Oben.Application.Products.Dtos;

namespace Oben.Application.Products.Commands.CreateProduct;

/// <summary>
/// Solicita la creacion de un producto con los campos editables definidos por la prueba.
/// </summary>
public sealed record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    int Stock) : IRequest<ProductDto>;

