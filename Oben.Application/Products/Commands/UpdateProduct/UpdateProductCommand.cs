using MediatR;
using Oben.Application.Products.Dtos;

namespace Oben.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Solicita la modificacion de un producto existente e incluye el usuario requerido por auditoria.
/// </summary>
public sealed record UpdateProductCommand(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    int UserId) : IRequest<ProductDto>;

