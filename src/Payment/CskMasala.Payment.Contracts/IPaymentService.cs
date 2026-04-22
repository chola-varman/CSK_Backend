namespace CskMasala.Payment.Contracts;

public interface IPaymentService
{
    Task<PaymentDto?> GetPaymentByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
