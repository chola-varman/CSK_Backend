using CskMasala.Payment.Contracts;
using CskMasala.Shared.Entities;

namespace CskMasala.Payment.Domain;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public decimal Amount { get; set; }
    public string? RazorpayOrderId { get; set; }
    public string? RazorpayPaymentId { get; set; }
    public string? RazorpaySignature { get; set; }
    public string? TransactionRef { get; set; }
}
