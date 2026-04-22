using CskMasala.Shared.Events;
using MediatR;

namespace CskMasala.Email.Application.Handlers;

public class OrderPlacedEmailHandler(EmailQueue queue) : INotificationHandler<OrderPlacedEvent>
{
    public async Task Handle(OrderPlacedEvent notification, CancellationToken ct)
    {
        var body = $"""
            <h2>Thank you for your order, {notification.UserName}!</h2>
            <p>Your order <strong>{notification.OrderId}</strong> has been placed successfully.</p>
            <p><strong>Total: ₹{notification.FinalAmount:F2}</strong></p>
            <p>We will notify you once your order is confirmed and shipped.</p>
            <br/><p>— CSK Masala Team</p>
            """;

        await queue.Writer.WriteAsync(
            new EmailMessage(notification.UserEmail, "Order Placed - CSK Masala", body), ct);
    }
}

public class PaymentCapturedEmailHandler(EmailQueue queue) : INotificationHandler<PaymentCapturedEvent>
{
    public async Task Handle(PaymentCapturedEvent notification, CancellationToken ct)
    {
        var body = $"""
            <h2>Payment Confirmed!</h2>
            <p>Your payment of <strong>₹{notification.Amount:F2}</strong> via {notification.PaymentMethod} has been captured.</p>
            <p>Order ID: <strong>{notification.OrderId}</strong></p>
            <p>Your order is now confirmed and will be processed shortly.</p>
            <br/><p>— CSK Masala Team</p>
            """;

        await queue.Writer.WriteAsync(
            new EmailMessage(notification.UserEmail, "Payment Confirmed - CSK Masala", body), ct);
    }
}
