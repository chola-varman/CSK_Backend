using CskMasala.Receipt.Contracts;
using CskMasala.Shared.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CskMasala.Receipt.Api.Controllers;

/// <summary>Receipt/invoice generation</summary>
[ApiController]
[Route("api/receipts")]
public class ReceiptsController(IReceiptService receiptService, IExecutionContext ctx) : ControllerBase
{
    /// <summary>Download PDF receipt for an order</summary>
    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetReceipt(Guid orderId, CancellationToken ct)
    {
        if (!ctx.IsAuthenticated) return Unauthorized();

        var pdf = await receiptService.GeneratePdfAsync(orderId, ct);
        return File(pdf, "application/pdf", $"receipt-{orderId}.pdf");
    }
}
