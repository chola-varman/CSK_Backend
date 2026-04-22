using CskMasala.Retail.Application.Mappings;
using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Redis;
using CskMasala.Shared.Repositories;
using MediatR;

namespace CskMasala.Retail.Application.Commands;

public record CreateProductCommand(
    string Name, string Description, decimal Price, int Stock,
    Guid CategoryId, List<string> ImageUrls, int WeightGrams) : IRequest<ProductDto>;

public record UpdateProductCommand(
    Guid Id, string Name, string Description, decimal Price, int Stock,
    Guid CategoryId, List<string> ImageUrls, int WeightGrams, bool IsActive) : IRequest<ProductDto>;

public record DeleteProductCommand(Guid Id) : IRequest;

public class CreateProductCommandHandler(IWriteRepository<Product> write, IRedisCache cache)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            ImageUrls = request.ImageUrls,
            WeightGrams = request.WeightGrams
        };
        await write.AddAsync(product, ct);
        await write.SaveChangesAsync(ct);
        await cache.RemoveAsync("products:all", ct);
        return product.ToDto();
    }
}

public class UpdateProductCommandHandler(
    IReadRepository<Product> read, IWriteRepository<Product> write, IRedisCache cache)
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await read.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Product not found");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;
        product.ImageUrls = request.ImageUrls;
        product.WeightGrams = request.WeightGrams;
        product.IsActive = request.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        write.Update(product);
        await write.SaveChangesAsync(ct);
        await cache.RemoveAsync("products:all", ct);
        await cache.RemoveAsync($"product:{request.Id}", ct);
        return product.ToDto();
    }
}

public class DeleteProductCommandHandler(
    IReadRepository<Product> read, IWriteRepository<Product> write, IRedisCache cache)
    : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await read.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Product not found");
        write.Delete(product);
        await write.SaveChangesAsync(ct);
        await cache.RemoveAsync("products:all", ct);
        await cache.RemoveAsync($"product:{request.Id}", ct);
    }
}
