using CskMasala.Payment.Application.Commands;
using CskMasala.Payment.Contracts;
using CskMasala.Shared.Repositories;

namespace CskMasala.Payment.Application.Services;

public class PaymentService(IReadRepository<Domain.Payment> read) : IPaymentService
{
    public async Task<PaymentDto?> GetPaymentByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        var payments = await read.FindAsync(p => p.OrderId == orderId, ct);
        return payments.FirstOrDefault()?.ToDto();
    }
}
