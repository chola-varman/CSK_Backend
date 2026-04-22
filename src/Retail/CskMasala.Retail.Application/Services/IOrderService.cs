using CskMasala.Retail.Contracts;

namespace CskMasala.Retail.Application.Services;

public interface IOrderService
{
    Task<List<OrderSummaryDto>> GetOrdersAsync(Guid userId, CancellationToken ct = default);
    Task<OrderSummaryDto?> GetOrderByIdAsync(Guid orderId, Guid userId, CancellationToken ct = default);
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status, CancellationToken ct = default);
}
