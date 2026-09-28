using MediatR;
using Oben.Application.Common.Exceptions;
using Oben.Application.Products.Contracts;
using Oben.Application.Products.Dtos;

namespace Oben.Application.Products.Queries.GetProductById;

/// <summary>
/// Obtiene un producto existente y devuelve un contrato de salida estable para API o UI.
/// </summary>
public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository productRepository;

    /// <summary>
    /// Recibe el repositorio como lectura abstracta para no exponer detalles de SQL.
    /// </summary>
    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    /// <summary>
    /// Devuelve el producto solicitado o una excepcion de aplicacion cuando no existe.
    /// </summary>
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Product with id {request.Id} was not found.");

        return ProductDto.FromDomain(product);
    }
}

