using CskMasala.Shared.Entities;

namespace CskMasala.Retail.Domain;

public class ShippingRate : BaseEntity
{
    public string RegionName { get; set; } = string.Empty;
    public int WeightFromGrams { get; set; }
    public int WeightToGrams { get; set; }
    public decimal Rate { get; set; }
}
