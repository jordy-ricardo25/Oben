namespace Oben.Api.Models.Products;

/// <summary>
/// Entrada HTTP para actualizar los campos editables de un producto.
/// </summary>
public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int Stock);
