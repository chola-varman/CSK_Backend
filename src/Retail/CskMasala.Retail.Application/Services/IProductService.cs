using CskMasala.Retail.Contracts;

namespace CskMasala.Retail.Application.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetProductsAsync(Guid? categoryId, CancellationToken ct = default);
    Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);
    Task<ProductDto> CreateProductAsync(string name, string description, decimal price, int stock, Guid categoryId, List<string> imageUrls, int weightGrams, CancellationToken ct = default);
    Task<ProductDto> UpdateProductAsync(Guid id, string name, string description, decimal price, int stock, Guid categoryId, List<string> imageUrls, int weightGrams, bool isActive, CancellationToken ct = default);
    Task DeleteProductAsync(Guid id, CancellationToken ct = default);
    Task<CategoryDto> CreateCategoryAsync(string name, string? description, string? imageUrl, CancellationToken ct = default);
}
