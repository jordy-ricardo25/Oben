using MediatR;
using Oben.Application.Common.Exceptions;
using Oben.Application.Products.Contracts;

namespace Oben.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Verifica la existencia del producto y delega la eliminacion al repositorio.
/// </summary>
public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository productRepository;

    /// <summary>
    /// Recibe el repositorio como contrato para aislar el caso de uso de la estrategia SQL.
    /// </summary>
    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    /// <summary>
    /// Elimina el producto o falla de forma explicita cuando no existe.
    /// </summary>
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _ = await productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Product with id {request.Id} was not found.");

        await productRepository.DeleteAsync(request.Id, request.UserId, cancellationToken);
    }
}

