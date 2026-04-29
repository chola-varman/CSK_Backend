using CskMasala.Retail.Application.Mappings;
using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Events;
using CskMasala.Shared.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CskMasala.Retail.Application.Commands;

public record PlaceOrderItemRequest(Guid ProductId, int Quantity);
public record PlaceOrderAddressRequest(string Line1, string? Line2, string City, string State, string PinCode);

public record PlaceOrderCommand(
    Guid UserId,
    string UserEmail,
    string UserName,
    List<PlaceOrderItemRequest> Items,
    PlaceOrderAddressRequest ShippingAddress,
    string? CouponCode) : IRequest<OrderSummaryDto>;

public class PlaceOrderCommandHandler(
    IReadRepository<Product> productRead,
    IWriteRepository<Product> productWrite,
    IReadRepository<Coupon> couponRead,
    IWriteRepository<Coupon> couponWrite,
    IReadRepository<ShippingRate> shippingRead,
    IWriteRepository<Order> orderWrite,
    IPublisher publisher)
    : IRequestHandler<PlaceOrderCommand, OrderSummaryDto>
{
    public async Task<OrderSummaryDto> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        var orderItems = new List<OrderItem>();
        var totalWeight = 0;
        var totalAmount = 0m;

        foreach (var item in request.Items)
        {
            var product = await productRead.GetByIdAsync(item.ProductId, ct)
                ?? throw new KeyNotFoundException($"Product {item.ProductId} not found");

            if (!product.IsActive)
                throw new InvalidOperationException($"Product '{product.Name}' is not available");

            if (product.Stock < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for '{product.Name}'");

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity,
                WeightGrams = product.WeightGrams
            });

            product.Stock -= item.Quantity;
            product.UpdatedAt = DateTime.UtcNow;
            productWrite.Update(product);

            totalWeight += product.WeightGrams * item.Quantity;
            totalAmount += product.Price * item.Quantity;
        }

        var shippingAmount = await CalculateShippingAsync(request.ShippingAddress.State, totalWeight, ct);

        var discountAmount = 0m;
        Guid? couponId = null;

        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var coupons = await couponRead.FindAsync(c => c.Code == request.CouponCode.ToUpperInvariant() && c.IsActive, ct);
            var coupon = coupons.FirstOrDefault();

            if (coupon != null && coupon.ExpiresAt > DateTime.UtcNow && totalAmount >= coupon.MinOrderAmount)
            {
                discountAmount = coupon.DiscountType == DiscountType.Percentage
                    ? Math.Round(totalAmount * coupon.DiscountValue / 100, 2)
                    : coupon.DiscountValue;

                coupon.UsedCount++;
                coupon.UpdatedAt = DateTime.UtcNow;
                couponWrite.Update(coupon);
                couponId = coupon.Id;
            }
        }

        var order = new Order
        {
            UserId = request.UserId,
            UserEmail = request.UserEmail,
            UserName = request.UserName,
            Items = orderItems,
            TotalAmount = totalAmount,
            DiscountAmount = discountAmount,
            ShippingAmount = shippingAmount,
            FinalAmount = totalAmount - discountAmount + shippingAmount,
            CouponId = couponId,
            ShippingAddress = new ShippingAddress
            {
                Line1 = request.ShippingAddress.Line1,
                Line2 = request.ShippingAddress.Line2,
                City = request.ShippingAddress.City,
                State = request.ShippingAddress.State,
                PinCode = request.ShippingAddress.PinCode
            }
        };

        await orderWrite.AddAsync(order, ct);
        await orderWrite.SaveChangesAsync(ct);

        await publisher.Publish(new OrderPlacedEvent(
            order.Id, order.UserId, order.UserEmail, order.UserName, order.FinalAmount), ct);

        return order.ToDto();
    }

    private async Task<decimal> CalculateShippingAsync(string region, int weightGrams, CancellationToken ct)
    {
        var rates = await shippingRead.FindAsync(
            r => r.RegionName == region && r.WeightFromGrams <= weightGrams && r.WeightToGrams >= weightGrams, ct);
        return rates.FirstOrDefault()?.Rate ?? 50m;
    }
}
