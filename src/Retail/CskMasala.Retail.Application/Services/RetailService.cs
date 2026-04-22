using CskMasala.Retail.Application.Mappings;
using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Repositories;

namespace CskMasala.Retail.Application.Services;

public class RetailService(
    IReadRepository<Order> orderRead,
    IWriteRepository<Order> orderWrite)
    : IRetailService
{
    public async Task<OrderSummaryDto?> GetOrderSummaryAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await orderRead.GetByIdAsync(orderId, ct);
        return order?.ToDto();
    }

    public async Task<bool> UpdateOrderPaymentStatusAsync(Guid orderId, OrderStatus status, CancellationToken ct = default)
    {
        var order = await orderRead.GetByIdAsync(orderId, ct);
        if (order is null) return false;
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        orderWrite.Update(order);
        await orderWrite.SaveChangesAsync(ct);
        return true;
    }
}
