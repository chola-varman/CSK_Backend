using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Redis;
using CskMasala.Shared.Repositories;
using MediatR;

namespace CskMasala.Retail.Application.Queries;

public record ValidateCouponQuery(string Code, decimal OrderAmount) : IRequest<CouponValidationDto>;

public class ValidateCouponQueryHandler(IReadRepository<Coupon> couponRead, IRedisCache cache)
    : IRequestHandler<ValidateCouponQuery, CouponValidationDto>
{
    public async Task<CouponValidationDto> Handle(ValidateCouponQuery request, CancellationToken ct)
    {
        var key = $"coupon:{request.Code}";
        var coupons = await couponRead.FindAsync(c => c.Code == request.Code && c.IsActive, ct);
        var coupon = coupons.FirstOrDefault();

        if (coupon is null)
            return new CouponValidationDto(false, 0, "Coupon not found");

        if (coupon.ExpiresAt < DateTime.UtcNow)
            return new CouponValidationDto(false, 0, "Coupon has expired");

        if (coupon.UsedCount >= coupon.MaxUsage)
            return new CouponValidationDto(false, 0, "Coupon usage limit reached");

        if (request.OrderAmount < coupon.MinOrderAmount)
            return new CouponValidationDto(false, 0, $"Minimum order amount is ₹{coupon.MinOrderAmount}");

        var discount = coupon.DiscountType == DiscountType.Percentage
            ? Math.Round(request.OrderAmount * coupon.DiscountValue / 100, 2)
            : coupon.DiscountValue;

        return new CouponValidationDto(true, discount, null);
    }
}
