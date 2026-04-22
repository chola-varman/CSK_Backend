using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Repositories;

namespace CskMasala.Retail.Application.Services;

public class CouponService(
    IReadRepository<Coupon> couponRead,
    IWriteRepository<Coupon> couponWrite) : ICouponService
{
    public async Task<CouponValidationDto> ValidateCouponAsync(string code, decimal orderAmount, CancellationToken ct = default)
    {
        var coupons = await couponRead.FindAsync(c => c.Code == code.ToUpperInvariant() && c.IsActive, ct);
        var coupon = coupons.FirstOrDefault();

        if (coupon is null)
            return new CouponValidationDto(false, 0, "Coupon not found");

        if (coupon.ExpiresAt < DateTime.UtcNow)
            return new CouponValidationDto(false, 0, "Coupon has expired");

        if (coupon.UsedCount >= coupon.MaxUsage)
            return new CouponValidationDto(false, 0, "Coupon usage limit reached");

        if (orderAmount < coupon.MinOrderAmount)
            return new CouponValidationDto(false, 0, $"Minimum order amount is ₹{coupon.MinOrderAmount}");

        var discount = coupon.DiscountType == DiscountType.Percentage
            ? Math.Round(orderAmount * coupon.DiscountValue / 100, 2)
            : coupon.DiscountValue;

        return new CouponValidationDto(true, discount, null);
    }

    public async Task<Guid> CreateCouponAsync(string code, DiscountType discountType, decimal discountValue,
        decimal minOrderAmount, int maxUsage, DateTime expiresAt, CancellationToken ct = default)
    {
        var coupon = new Coupon
        {
            Code = code.ToUpperInvariant(),
            DiscountType = discountType,
            DiscountValue = discountValue,
            MinOrderAmount = minOrderAmount,
            MaxUsage = maxUsage,
            ExpiresAt = expiresAt
        };
        await couponWrite.AddAsync(coupon, ct);
        await couponWrite.SaveChangesAsync(ct);
        return coupon.Id;
    }
}
