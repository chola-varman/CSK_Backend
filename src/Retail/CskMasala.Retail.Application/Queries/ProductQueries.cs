using CskMasala.Retail.Application.Mappings;
using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Redis;
using CskMasala.Shared.Repositories;
using MediatR;

namespace CskMasala.Retail.Application.Queries;

public record GetProductsQuery(Guid? CategoryId = null) : IRequest<List<ProductDto>>;
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

public class GetProductsQueryHandler(IReadRepository<Product> productRead, IRedisCache cache)
    : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var key = $"products:{request.CategoryId?.ToString() ?? "all"}";
        var cached = await cache.GetAsync<List<ProductDto>>(key, ct);
        if (cached != null) return cached;

        var products = await productRead.FindAsync(
            p => p.IsActive && (request.CategoryId == null || p.CategoryId == request.CategoryId), ct);

        var dtos = products.Select(p => p.ToDto()).ToList();
        await cache.SetAsync(key, dtos, TimeSpan.FromMinutes(30), ct);
        return dtos;
    }
}

public class GetProductByIdQueryHandler(IReadRepository<Product> productRead, IRedisCache cache)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var key = $"product:{request.Id}";
        var cached = await cache.GetAsync<ProductDto>(key, ct);
        if (cached != null) return cached;

        var product = await productRead.GetByIdAsync(request.Id, ct);
        if (product is null || !product.IsActive) return null;

        var dto = product.ToDto();
        await cache.SetAsync(key, dto, TimeSpan.FromMinutes(30), ct);
        return dto;
    }
}

public class GetCategoriesQueryHandler(IReadRepository<Category> categoryRead)
    : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var categories = await categoryRead.GetAllAsync(ct);
        return categories.Select(c => c.ToDto()).ToList();
    }
}
