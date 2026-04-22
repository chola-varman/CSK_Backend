using CskMasala.Retail.Application.Services;
using CskMasala.Retail.Contracts;
using CskMasala.Shared.Auth;
using CskMasala.Shared.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CskMasala.Retail.Api.Controllers;

/// <summary>Coupon management</summary>
[ApiController]
[Route("api/coupons")]
public class CouponsController(ICouponService couponService, IExecutionContext ctx) : ControllerBase
{
    /// <summary>Validate a coupon code against an order amount</summary>
    [HttpGet("{code}/validate")]
    public async Task<IActionResult> ValidateCoupon(string code, [FromQuery] decimal orderAmount, CancellationToken ct) =>
        Ok(await couponService.ValidateCouponAsync(code, orderAmount, ct));

    /// <summary>Create a new coupon (Admin only)</summary>
    [HttpPost]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponRequest request, CancellationToken ct)
    {
        if (ctx.Role != UserRole.Admin) return Forbid();
        var id = await couponService.CreateCouponAsync(request.Code, request.DiscountType, request.DiscountValue,
            request.MinOrderAmount, request.MaxUsage, request.ExpiresAt, ct);
        return Ok(new { id });
    }
}

public record CreateCouponRequest(
    string Code,
    DiscountType DiscountType,
    decimal DiscountValue,
    decimal MinOrderAmount,
    int MaxUsage,
    DateTime ExpiresAt);
