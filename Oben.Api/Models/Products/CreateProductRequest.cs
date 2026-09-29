namespace Oben.Api.Models.Products;

/// <summary>
/// Entrada HTTP para crear un producto.
/// </summary>
public sealed record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int Stock);
