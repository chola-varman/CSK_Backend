using CskMasala.Payment.Contracts;
using CskMasala.Payment.Domain;
using CskMasala.Retail.Contracts;
using CskMasala.Shared.Events;
using CskMasala.Shared.Repositories;
using MediatR;

namespace CskMasala.Payment.Application.Commands;

// Razorpay: create order
public record CreateRazorpayOrderCommand(Guid OrderId, Guid UserId, string UserEmail, decimal Amount) : IRequest<RazorpayOrderResult>;
public record RazorpayOrderResult(string RazorpayOrderId, decimal Amount);

// Razorpay: verify payment
public record VerifyRazorpayPaymentCommand(
    Guid OrderId, string RazorpayOrderId, string RazorpayPaymentId, string Signature,
    Guid UserId, string UserEmail) : IRequest<PaymentDto>;

// COD: instant capture
public record ProcessCodPaymentCommand(Guid OrderId, Guid UserId, string UserEmail, decimal Amount) : IRequest<PaymentDto>;

public class CreateRazorpayOrderCommandHandler(
    IWriteRepository<Domain.Payment> write,
    IReadRepository<Domain.Payment> read,
    RazorpayClient razorpay)
    : IRequestHandler<CreateRazorpayOrderCommand, RazorpayOrderResult>
{
    public async Task<RazorpayOrderResult> Handle(CreateRazorpayOrderCommand request, CancellationToken ct)
    {
        var rzpOrderId = await razorpay.CreateOrderAsync(request.Amount, request.OrderId.ToString(), ct);

        var payment = new Domain.Payment
        {
            OrderId = request.OrderId,
            UserId = request.UserId,
            Method = PaymentMethod.Razorpay,
            Status = PaymentStatus.Pending,
            Amount = request.Amount,
            RazorpayOrderId = rzpOrderId
        };
        await write.AddAsync(payment, ct);
        await write.SaveChangesAsync(ct);

        return new RazorpayOrderResult(rzpOrderId, request.Amount);
    }
}

public class VerifyRazorpayPaymentCommandHandler(
    IReadRepository<Domain.Payment> read,
    IWriteRepository<Domain.Payment> write,
    IRetailService retailService,
    RazorpayClient razorpay,
    IPublisher publisher)
    : IRequestHandler<VerifyRazorpayPaymentCommand, PaymentDto>
{
    public async Task<PaymentDto> Handle(VerifyRazorpayPaymentCommand request, CancellationToken ct)
    {
        var payments = await read.FindAsync(p => p.OrderId == request.OrderId && p.RazorpayOrderId == request.RazorpayOrderId, ct);
        var payment = payments.FirstOrDefault()
            ?? throw new KeyNotFoundException("Payment record not found");

        if (!razorpay.VerifySignature(request.RazorpayOrderId, request.RazorpayPaymentId, request.Signature))
        {
            payment.Status = PaymentStatus.Failed;
            payment.UpdatedAt = DateTime.UtcNow;
            write.Update(payment);
            await write.SaveChangesAsync(ct);
            throw new InvalidOperationException("Payment signature verification failed");
        }

        payment.Status = PaymentStatus.Captured;
        payment.RazorpayPaymentId = request.RazorpayPaymentId;
        payment.RazorpaySignature = request.Signature;
        payment.TransactionRef = request.RazorpayPaymentId;
        payment.UpdatedAt = DateTime.UtcNow;
        write.Update(payment);
        await write.SaveChangesAsync(ct);

        await retailService.UpdateOrderPaymentStatusAsync(request.OrderId, OrderStatus.Confirmed, ct);
        await publisher.Publish(new PaymentCapturedEvent(
            request.OrderId, request.UserId, request.UserEmail, "Razorpay", payment.Amount), ct);

        return payment.ToDto();
    }
}

public class ProcessCodPaymentCommandHandler(
    IWriteRepository<Domain.Payment> write,
    IRetailService retailService,
    IPublisher publisher)
    : IRequestHandler<ProcessCodPaymentCommand, PaymentDto>
{
    public async Task<PaymentDto> Handle(ProcessCodPaymentCommand request, CancellationToken ct)
    {
        var payment = new Domain.Payment
        {
            OrderId = request.OrderId,
            UserId = request.UserId,
            Method = PaymentMethod.COD,
            Status = PaymentStatus.Captured,
            Amount = request.Amount,
            TransactionRef = "COD"
        };
        await write.AddAsync(payment, ct);
        await write.SaveChangesAsync(ct);

        await retailService.UpdateOrderPaymentStatusAsync(request.OrderId, OrderStatus.Confirmed, ct);
        await publisher.Publish(new PaymentCapturedEvent(
            request.OrderId, request.UserId, request.UserEmail, "COD", request.Amount), ct);

        return payment.ToDto();
    }
}

internal static class PaymentMappings
{
    internal static PaymentDto ToDto(this Domain.Payment p) => new(
        p.Id, p.OrderId, p.Method.ToString(), p.Status.ToString(),
        p.Amount, p.TransactionRef, p.CreatedAt);
}
