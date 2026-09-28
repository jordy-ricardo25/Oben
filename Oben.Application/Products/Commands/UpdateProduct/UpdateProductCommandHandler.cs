using MediatR;
using Oben.Application.Common.Exceptions;
using Oben.Application.Products.Contracts;
using Oben.Application.Products.Dtos;

namespace Oben.Application.Products.Commands.UpdateProduct;

/// <summary>
/// Recupera el producto, aplica las reglas de dominio y solicita la persistencia del cambio.
/// </summary>
public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository productRepository;

    /// <summary>
    /// Recibe el repositorio como dependencia abstracta para mantener aislada la persistencia.
    /// </summary>
    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    /// <summary>
    /// Actualiza todos los campos editables o falla si el producto no existe.
    /// </summary>
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Product with id {request.Id} was not found.");

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.Stock);

        await productRepository.UpdateAsync(product, request.UserId, cancellationToken);

        return ProductDto.FromDomain(product);
    }
}

