using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;

namespace CskMasala.Retail.Application.Mappings;

internal static class RetailMappings
{
    internal static ProductDto ToDto(this Product p) => new(
        p.Id, p.Name, p.Description, p.Price, p.Stock,
        p.CategoryId, p.Category?.Name ?? string.Empty,
        p.ImageUrls, p.WeightGrams, p.IsActive);

    internal static CategoryDto ToDto(this Category c) => new(c.Id, c.Name, c.Description, c.ImageUrl);

    internal static OrderSummaryDto ToDto(this Order o) => new(
        o.Id, o.UserId, o.UserEmail, o.UserName,
        o.Status.ToString(),
        o.TotalAmount, o.DiscountAmount, o.ShippingAmount, o.FinalAmount,
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.UnitPrice * i.Quantity)).ToList(),
        new ShippingAddressDto(o.ShippingAddress.Line1, o.ShippingAddress.Line2, o.ShippingAddress.City, o.ShippingAddress.State, o.ShippingAddress.PinCode),
        o.CreatedAt);
}
