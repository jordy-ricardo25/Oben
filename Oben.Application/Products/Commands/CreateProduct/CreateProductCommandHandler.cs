using MediatR;
using Oben.Application.Products.Contracts;
using Oben.Application.Products.Dtos;
using Oben.Domain.Entities;

namespace Oben.Application.Products.Commands.CreateProduct;

/// <summary>
/// Crea la entidad de dominio y delega la persistencia al contrato de productos.
/// </summary>
public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository productRepository;

    /// <summary>
    /// Recibe el repositorio como puerto de aplicacion para no acoplar el caso de uso a SQL Server.
    /// </summary>
    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    /// <summary>
    /// Construye un producto valido y devuelve el DTO con el identificador generado.
    /// </summary>
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            id: 0,
            name: request.Name,
            description: request.Description,
            price: request.Price,
            stock: request.Stock);

        var createdProduct = await productRepository.AddAsync(product, cancellationToken);

        return ProductDto.FromDomain(createdProduct);
    }
}
