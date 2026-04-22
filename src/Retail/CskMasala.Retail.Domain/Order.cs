using CskMasala.Retail.Contracts;
using CskMasala.Shared.Entities;

namespace CskMasala.Retail.Domain;

public class Order : BaseEntity
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Ordered;
    public List<OrderItem> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
    public Guid? CouponId { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public ShippingAddress ShippingAddress { get; set; } = null!;
}

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int WeightGrams { get; set; }
}

public class ShippingAddress
{
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PinCode { get; set; } = string.Empty;
}
