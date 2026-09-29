using MediatR;

namespace Oben.Application.Products.Commands.DeleteProduct;

/// <summary>
/// Solicita eliminar un producto e incluye el usuario requerido por los triggers de auditoria.
/// </summary>
public sealed record DeleteProductCommand(int Id, int UserId) : IRequest;

