using CskMasala.Retail.Contracts;

namespace CskMasala.Retail.Application.Services;

public interface ICouponService
{
    Task<CouponValidationDto> ValidateCouponAsync(string code, decimal orderAmount, CancellationToken ct = default);
    Task<Guid> CreateCouponAsync(string code, DiscountType discountType, decimal discountValue, decimal minOrderAmount, DateTime expiresAt, CancellationToken ct = default);
}
