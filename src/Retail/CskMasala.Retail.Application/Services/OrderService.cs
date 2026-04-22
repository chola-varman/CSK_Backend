using CskMasala.Retail.Application.Mappings;
using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Repositories;

namespace CskMasala.Retail.Application.Services;

public class OrderService(
    IReadRepository<Order> orderRead,
    IWriteRepository<Order> orderWrite) : IOrderService
{
    public async Task<List<OrderSummaryDto>> GetOrdersAsync(Guid userId, CancellationToken ct = default)
    {
        var orders = await orderRead.FindAsync(o => o.UserId == userId, ct);
        return orders.Select(o => o.ToDto()).ToList();
    }

    public async Task<OrderSummaryDto?> GetOrderByIdAsync(Guid orderId, Guid userId, CancellationToken ct = default)
    {
        var order = await orderRead.GetByIdAsync(orderId, ct);
        if (order is null || order.UserId != userId) return null;
        return order.ToDto();
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status, CancellationToken ct = default)
    {
        var order = await orderRead.GetByIdAsync(orderId, ct)
            ?? throw new KeyNotFoundException("Order not found");

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        orderWrite.Update(order);
        await orderWrite.SaveChangesAsync(ct);
    }
}
