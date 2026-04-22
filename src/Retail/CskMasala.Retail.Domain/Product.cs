using CskMasala.Shared.Entities;

namespace CskMasala.Retail.Domain;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public List<string> ImageUrls { get; set; } = [];
    public int WeightGrams { get; set; }
    public bool IsActive { get; set; } = true;
}
