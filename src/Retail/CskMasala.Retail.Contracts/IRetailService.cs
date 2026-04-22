namespace CskMasala.Retail.Contracts;

public interface IRetailService
{
    Task<OrderSummaryDto?> GetOrderSummaryAsync(Guid orderId, CancellationToken ct = default);
    Task<bool> UpdateOrderPaymentStatusAsync(Guid orderId, OrderStatus status, CancellationToken ct = default);
}
