using CskMasala.Payment.Application.Commands;
using CskMasala.Retail.Contracts;
using CskMasala.Shared.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CskMasala.Payment.Api.Controllers;

/// <summary>Payment processing</summary>
[ApiController]
[Route("api/payments")]
public class PaymentsController(IMediator mediator, IExecutionContext ctx, IRetailService retailService) : ControllerBase
{
    /// <summary>Create a Razorpay order for payment</summary>
    [HttpPost("razorpay/create-order")]
    public async Task<IActionResult> CreateRazorpayOrder([FromBody] CreateRazorpayOrderRequest request, CancellationToken ct)
    {
        if (!ctx.IsAuthenticated) return Unauthorized();

        var order = await retailService.GetOrderSummaryAsync(request.OrderId, ct)
            ?? throw new KeyNotFoundException("Order not found");

        var result = await mediator.Send(
            new CreateRazorpayOrderCommand(request.OrderId, ctx.UserId, ctx.Email, order.FinalAmount), ct);
        return Ok(result);
    }

    /// <summary>Verify Razorpay payment signature and capture</summary>
    [HttpPost("razorpay/verify")]
    public async Task<IActionResult> VerifyRazorpayPayment([FromBody] VerifyRazorpayPaymentRequest request, CancellationToken ct)
    {
        if (!ctx.IsAuthenticated) return Unauthorized();

        var result = await mediator.Send(new VerifyRazorpayPaymentCommand(
            request.OrderId, request.RazorpayOrderId, request.RazorpayPaymentId,
            request.Signature, ctx.UserId, ctx.Email), ct);
        return Ok(result);
    }

    /// <summary>Confirm Cash on Delivery payment</summary>
    [HttpPost("cod")]
    public async Task<IActionResult> ProcessCod([FromBody] CodPaymentRequest request, CancellationToken ct)
    {
        if (!ctx.IsAuthenticated) return Unauthorized();

        var order = await retailService.GetOrderSummaryAsync(request.OrderId, ct)
            ?? throw new KeyNotFoundException("Order not found");

        var result = await mediator.Send(
            new ProcessCodPaymentCommand(request.OrderId, ctx.UserId, ctx.Email, order.FinalAmount), ct);
        return Ok(result);
    }
}

public record CreateRazorpayOrderRequest(Guid OrderId);
public record VerifyRazorpayPaymentRequest(Guid OrderId, string RazorpayOrderId, string RazorpayPaymentId, string Signature);
public record CodPaymentRequest(Guid OrderId);
