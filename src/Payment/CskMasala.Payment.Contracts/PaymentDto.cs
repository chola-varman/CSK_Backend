namespace CskMasala.Payment.Contracts;

public enum PaymentMethod { Razorpay, COD, UPI }
public enum PaymentStatus { Pending, Captured, Failed, Refunded }

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    string Method,
    string Status,
    decimal Amount,
    string? TransactionRef,
    DateTime CreatedAt);
