using MediatR;
using Oben.Application.Products.Contracts;
using Oben.Application.Products.Dtos;

namespace Oben.Application.Products.Queries.GetProducts;

/// <summary>
/// Obtiene todos los productos y los proyecta al contrato de salida de aplicacion.
/// </summary>
public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyCollection<ProductDto>>
{
    private readonly IProductRepository productRepository;

    /// <summary>
    /// Recibe el repositorio como contrato de lectura para mantener la query libre de SQL.
    /// </summary>
    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    /// <summary>
    /// Devuelve el listado actual de productos. No aplica paginacion porque esta fuera del alcance definido.
    /// </summary>
    public async Task<IReadOnlyCollection<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);

        return products
            .Select(ProductDto.FromDomain)
            .ToList();
    }
}

