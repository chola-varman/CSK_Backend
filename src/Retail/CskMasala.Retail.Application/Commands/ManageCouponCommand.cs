using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Repositories;
using MediatR;

namespace CskMasala.Retail.Application.Commands;

public record CreateCouponCommand(
    string Code,
    DiscountType DiscountType,
    decimal DiscountValue,
    decimal MinOrderAmount,
    int MaxUsage,
    DateTime ExpiresAt) : IRequest<Guid>;

public class CreateCouponCommandHandler(IWriteRepository<Coupon> write)
    : IRequestHandler<CreateCouponCommand, Guid>
{
    public async Task<Guid> Handle(CreateCouponCommand request, CancellationToken ct)
    {
        var coupon = new Coupon
        {
            Code = request.Code.ToUpperInvariant(),
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            MinOrderAmount = request.MinOrderAmount,
            MaxUsage = request.MaxUsage,
            ExpiresAt = request.ExpiresAt
        };
        await write.AddAsync(coupon, ct);
        await write.SaveChangesAsync(ct);
        return coupon.Id;
    }
}
