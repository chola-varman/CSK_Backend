using CskMasala.Retail.Application.Commands;
using CskMasala.Retail.Application.Queries;
using CskMasala.Retail.Contracts;
using CskMasala.Shared.Auth;
using CskMasala.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CskMasala.Retail.Api.Controllers;

/// <summary>Product and category management</summary>
[ApiController]
[Route("api")]
public class ProductsController(IMediator mediator, IExecutionContext ctx) : ControllerBase
{
    /// <summary>Get all products, optionally filtered by category</summary>
    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] Guid? categoryId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetProductsQuery(categoryId), ct));

    /// <summary>Get a product by ID</summary>
    [HttpGet("products/{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Create a new product (Admin only)</summary>
    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        if (ctx.Role != UserRole.Admin) return Forbid();
        var cmd = new CreateProductCommand(request.Name, request.Description, request.Price, request.Stock,
            request.CategoryId, request.ImageUrls, request.WeightGrams);
        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    /// <summary>Update a product (Admin only)</summary>
    [HttpPut("products/{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        if (ctx.Role != UserRole.Admin) return Forbid();
        var cmd = new UpdateProductCommand(id, request.Name, request.Description, request.Price,
            request.Stock, request.CategoryId, request.ImageUrls, request.WeightGrams, request.IsActive);
        return Ok(await mediator.Send(cmd, ct));
    }

    /// <summary>Delete a product (Admin only)</summary>
    [HttpDelete("products/{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct)
    {
        if (ctx.Role != UserRole.Admin) return Forbid();
        await mediator.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }

    /// <summary>Get all categories</summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken ct) =>
        Ok(await mediator.Send(new GetCategoriesQuery(), ct));

    /// <summary>Create a new category (Admin only)</summary>
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        if (ctx.Role != UserRole.Admin) return Forbid();
        var result = await mediator.Send(new CreateCategoryCommand(request.Name, request.Description, request.ImageUrl), ct);
        return Ok(result);
    }
}

public record CreateProductRequest(string Name, string Description, decimal Price, int Stock,
    Guid CategoryId, List<string> ImageUrls, int WeightGrams);
public record UpdateProductRequest(string Name, string Description, decimal Price, int Stock,
    Guid CategoryId, List<string> ImageUrls, int WeightGrams, bool IsActive);
public record CreateCategoryRequest(string Name, string? Description, string? ImageUrl);
