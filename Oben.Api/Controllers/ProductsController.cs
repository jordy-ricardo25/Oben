using MediatR;
using Microsoft.AspNetCore.Mvc;
using Oben.Api.Models.Products;
using Oben.Application.Products.Commands.CreateProduct;
using Oben.Application.Products.Commands.DeleteProduct;
using Oben.Application.Products.Commands.UpdateProduct;
using Oben.Application.Products.Dtos;
using Oben.Application.Products.Queries.GetProductById;
using Oben.Application.Products.Queries.GetProducts;

namespace Oben.Api.Controllers;

/// <summary>
/// Endpoints HTTP para el CRUD de productos.
/// </summary>
[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender sender;

    /// <summary>
    /// Recibe MediatR para delegar cada operacion en su command o query.
    /// </summary>
    public ProductsController(ISender sender)
    {
        this.sender = sender;
    }

    /// <summary>
    /// Obtiene el listado completo de productos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await sender.Send(new GetProductsQuery(), cancellationToken);

        return Ok(products);
    }

    /// <summary>
    /// Obtiene un producto por identificador.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await sender.Send(new GetProductByIdQuery(id), cancellationToken);

        return Ok(product);
    }

    /// <summary>
    /// Crea un producto nuevo.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await sender.Send(
            new CreateProductCommand(
                request.Name,
                request.Description,
                request.Price,
                request.Stock),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Actualiza un producto existente. El header X-User-Id alimenta la auditoria de SQL Server.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> Update(
        int id,
        [FromHeader(Name = "X-User-Id")] int userId,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await sender.Send(
            new UpdateProductCommand(
                id,
                request.Name,
                request.Description,
                request.Price,
                request.Stock,
                userId),
            cancellationToken);

        return Ok(product);
    }

    /// <summary>
    /// Elimina un producto existente. El header X-User-Id alimenta la auditoria de SQL Server.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        [FromHeader(Name = "X-User-Id")] int userId,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteProductCommand(id, userId), cancellationToken);

        return NoContent();
    }
}
