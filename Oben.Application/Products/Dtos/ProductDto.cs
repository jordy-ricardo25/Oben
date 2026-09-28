using Oben.Domain.Entities;

namespace Oben.Application.Products.Dtos;

/// <summary>
/// Modelo de salida para exponer productos desde aplicacion sin filtrar la entidad de dominio.
/// </summary>
public sealed record ProductDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock)
{
    /// <summary>
    /// Construye el DTO desde dominio con mapeo manual y visible.
    /// </summary>
    public static ProductDto FromDomain(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock);
    }
}

