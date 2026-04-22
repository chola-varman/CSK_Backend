using CskMasala.Retail.Application.Commands;
using CskMasala.Retail.Application.Services;
using CskMasala.Retail.Contracts;
using CskMasala.Shared.Auth;
using CskMasala.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CskMasala.Retail.Api.Controllers;

/// <summary>Order management</summary>
[ApiController]
[Route("api/orders")]
public class OrdersController(IMediator mediator, IOrderService orderService, IExecutionContext ctx) : ControllerBase
{
    /// <summary>Place a new order</summary>
    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request, CancellationToken ct)
    {
        if (!ctx.IsAuthenticated) return Unauthorized();

        var cmd = new PlaceOrderCommand(
            ctx.UserId, ctx.Email, string.Empty,
            request.Items.Select(i => new PlaceOrderItemRequest(i.ProductId, i.Quantity)).ToList(),
            new PlaceOrderAddressRequest(request.ShippingAddress.Line1, request.ShippingAddress.Line2,
                request.ShippingAddress.City, request.ShippingAddress.State, request.ShippingAddress.PinCode),
            request.CouponCode);

        var result = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
    }

    /// <summary>Get all orders for the current user</summary>
    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken ct)
    {
        if (!ctx.IsAuthenticated) return Unauthorized();
        return Ok(await orderService.GetOrdersAsync(ctx.UserId, ct));
    }

    /// <summary>Get a specific order</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken ct)
    {
        if (!ctx.IsAuthenticated) return Unauthorized();
        var result = await orderService.GetOrderByIdAsync(id, ctx.UserId, ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Update order status (Admin only)</summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request, CancellationToken ct)
    {
        if (ctx.Role != UserRole.Admin) return Forbid();
        await orderService.UpdateOrderStatusAsync(id, request.Status, ct);
        return NoContent();
    }
}

public record OrderItemRequest(Guid ProductId, int Quantity);
public record ShippingAddressRequest(string Line1, string? Line2, string City, string State, string PinCode);
public record PlaceOrderRequest(List<OrderItemRequest> Items, ShippingAddressRequest ShippingAddress, string? CouponCode);
public record UpdateOrderStatusRequest(OrderStatus Status);
