using System.ComponentModel.DataAnnotations;
using CskMasala.Retail.Contracts;
using CskMasala.Shared.Entities;

namespace CskMasala.Retail.Domain;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinOrderAmount { get; set; }
    [ConcurrencyCheck]
    public int UsedCount { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
}
