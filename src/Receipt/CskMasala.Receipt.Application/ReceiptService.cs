using CskMasala.Payment.Contracts;
using CskMasala.Receipt.Contracts;
using CskMasala.Retail.Contracts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CskMasala.Receipt.Application;

public class ReceiptService(IRetailService retailService, IPaymentService paymentService) : IReceiptService
{
    public async Task<byte[]> GeneratePdfAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await retailService.GetOrderSummaryAsync(orderId, ct)
            ?? throw new KeyNotFoundException("Order not found");
        var payment = await paymentService.GetPaymentByOrderIdAsync(orderId, ct);

        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text("CSK Masala").Bold().FontSize(20);
                    col.Item().Text("Tax Invoice").FontSize(14);
                    col.Item().Text($"Order #: {order.Id}").FontSize(10);
                    col.Item().Text($"Date: {order.CreatedAt:dd MMM yyyy}").FontSize(10);
                });

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text($"Customer: {order.UserName}").SemiBold();
                    col.Item().Text($"Email: {order.UserEmail}");
                    col.Item().Text($"Address: {order.ShippingAddress.Line1}, {order.ShippingAddress.City}, {order.ShippingAddress.State} - {order.ShippingAddress.PinCode}");
                    col.Item().PaddingTop(10).LineHorizontal(1);

                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Product").Bold();
                            header.Cell().Text("Qty").Bold();
                            header.Cell().Text("Unit Price").Bold();
                            header.Cell().Text("Subtotal").Bold();
                        });

                        foreach (var item in order.Items)
                        {
                            table.Cell().Text(item.ProductName);
                            table.Cell().Text(item.Quantity.ToString());
                            table.Cell().Text($"₹{item.UnitPrice:F2}");
                            table.Cell().Text($"₹{item.Subtotal:F2}");
                        }
                    });

                    col.Item().PaddingTop(10).LineHorizontal(1);
                    col.Item().AlignRight().Text($"Subtotal: ₹{order.TotalAmount:F2}");
                    col.Item().AlignRight().Text($"Discount: -₹{order.DiscountAmount:F2}");
                    col.Item().AlignRight().Text($"Shipping: ₹{order.ShippingAmount:F2}");
                    col.Item().AlignRight().Text($"Total: ₹{order.FinalAmount:F2}").Bold().FontSize(13);

                    if (payment != null)
                    {
                        col.Item().PaddingTop(10).Text($"Payment: {payment.Method} ({payment.Status})").FontSize(10);
                        if (payment.TransactionRef != null)
                            col.Item().Text($"Ref: {payment.TransactionRef}").FontSize(10);
                    }
                });

                page.Footer().AlignCenter().Text("Thank you for shopping with CSK Masala!").FontSize(10);
            });
        }).GeneratePdf();
    }
}
