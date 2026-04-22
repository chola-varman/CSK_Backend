using CskMasala.Retail.Application.Mappings;
using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Repositories;
using MediatR;

namespace CskMasala.Retail.Application.Queries;

public record GetOrdersQuery(Guid UserId) : IRequest<List<OrderSummaryDto>>;
public record GetOrderByIdQuery(Guid OrderId, Guid UserId) : IRequest<OrderSummaryDto?>;

public class GetOrdersQueryHandler(IReadRepository<Order> orderRead)
    : IRequestHandler<GetOrdersQuery, List<OrderSummaryDto>>
{
    public async Task<List<OrderSummaryDto>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        var orders = await orderRead.FindAsync(o => o.UserId == request.UserId, ct);
        return orders.Select(o => o.ToDto()).ToList();
    }
}

public class GetOrderByIdQueryHandler(IReadRepository<Order> orderRead)
    : IRequestHandler<GetOrderByIdQuery, OrderSummaryDto?>
{
    public async Task<OrderSummaryDto?> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await orderRead.GetByIdAsync(request.OrderId, ct);
        if (order is null || order.UserId != request.UserId) return null;
        return order.ToDto();
    }
}
