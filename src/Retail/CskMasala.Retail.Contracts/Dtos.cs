namespace CskMasala.Retail.Contracts;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId,
    string CategoryName,
    List<string> ImageUrls,
    int WeightGrams,
    bool IsActive);

public record CategoryDto(Guid Id, string Name, string? Description, string? ImageUrl);

public record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal Subtotal);

public record ShippingAddressDto(string Line1, string? Line2, string City, string State, string PinCode);

public record OrderSummaryDto(
    Guid Id,
    Guid UserId,
    string UserEmail,
    string UserName,
    string Status,
    decimal TotalAmount,
    decimal DiscountAmount,
    decimal ShippingAmount,
    decimal FinalAmount,
    List<OrderItemDto> Items,
    ShippingAddressDto ShippingAddress,
    DateTime CreatedAt);

public record CouponValidationDto(bool IsValid, decimal DiscountAmount, string? Message);
