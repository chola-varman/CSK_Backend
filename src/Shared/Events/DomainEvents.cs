using MediatR;

namespace CskMasala.Shared.Events;

public record OrderPlacedEvent(
    Guid OrderId,
    Guid UserId,
    string UserEmail,
    string UserName,
    decimal FinalAmount) : INotification;

public record PaymentCapturedEvent(
    Guid OrderId,
    Guid UserId,
    string UserEmail,
    string PaymentMethod,
    decimal Amount) : INotification;
