using CskMasala.Retail.Application.Mappings;
using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Redis;
using CskMasala.Shared.Repositories;

namespace CskMasala.Retail.Application.Services;

public class ProductService(
    IReadRepository<Product> productRead,
    IWriteRepository<Product> productWrite,
    IReadRepository<Category> categoryRead,
    IWriteRepository<Category> categoryWrite,
    IRedisCache cache) : IProductService
{
    public async Task<List<ProductDto>> GetProductsAsync(Guid? categoryId, CancellationToken ct = default)
    {
        var key = $"products:{categoryId?.ToString() ?? "all"}";
        var cached = await cache.GetAsync<List<ProductDto>>(key, ct);
        if (cached != null) return cached;

        var products = await productRead.FindAsync(
            p => p.IsActive && (categoryId == null || p.CategoryId == categoryId), ct);

        var dtos = products.Select(p => p.ToDto()).ToList();
        await cache.SetAsync(key, dtos, TimeSpan.FromMinutes(30), ct);
        return dtos;
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken ct = default)
    {
        var key = $"product:{id}";
        var cached = await cache.GetAsync<ProductDto>(key, ct);
        if (cached != null) return cached;

        var product = await productRead.GetByIdAsync(id, ct);
        if (product is null || !product.IsActive) return null;

        var dto = product.ToDto();
        await cache.SetAsync(key, dto, TimeSpan.FromMinutes(30), ct);
        return dto;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var categories = await categoryRead.GetAllAsync(ct);
        return categories.Select(c => c.ToDto()).ToList();
    }

    public async Task<ProductDto> CreateProductAsync(string name, string description, decimal price, int stock,
        Guid categoryId, List<string> imageUrls, int weightGrams, CancellationToken ct = default)
    {
        var product = new Product
        {
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
            CategoryId = categoryId,
            ImageUrls = imageUrls,
            WeightGrams = weightGrams
        };
        await productWrite.AddAsync(product, ct);
        await productWrite.SaveChangesAsync(ct);
        await cache.RemoveAsync("products:all", ct);
        return product.ToDto();
    }

    public async Task<ProductDto> UpdateProductAsync(Guid id, string name, string description, decimal price, int stock,
        Guid categoryId, List<string> imageUrls, int weightGrams, bool isActive, CancellationToken ct = default)
    {
        var product = await productRead.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");

        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.Stock = stock;
        product.CategoryId = categoryId;
        product.ImageUrls = imageUrls;
        product.WeightGrams = weightGrams;
        product.IsActive = isActive;
        product.UpdatedAt = DateTime.UtcNow;

        productWrite.Update(product);
        await productWrite.SaveChangesAsync(ct);
        await cache.RemoveAsync("products:all", ct);
        await cache.RemoveAsync($"product:{id}", ct);
        return product.ToDto();
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken ct = default)
    {
        var product = await productRead.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException("Product not found");
        productWrite.Delete(product);
        await productWrite.SaveChangesAsync(ct);
        await cache.RemoveAsync("products:all", ct);
        await cache.RemoveAsync($"product:{id}", ct);
    }

    public async Task<CategoryDto> CreateCategoryAsync(string name, string? description, string? imageUrl, CancellationToken ct = default)
    {
        var category = new Category
        {
            Name = name,
            Description = description,
            ImageUrl = imageUrl
        };
        await categoryWrite.AddAsync(category, ct);
        await categoryWrite.SaveChangesAsync(ct);
        return category.ToDto();
    }
}
