using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Repositories;
using MediatR;

namespace CskMasala.Retail.Application.Commands;

public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status) : IRequest;

public class UpdateOrderStatusCommandHandler(
    IReadRepository<Order> orderRead,
    IWriteRepository<Order> orderWrite)
    : IRequestHandler<UpdateOrderStatusCommand>
{
    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await orderRead.GetByIdAsync(request.OrderId, ct)
            ?? throw new KeyNotFoundException("Order not found");

        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;
        orderWrite.Update(order);
        await orderWrite.SaveChangesAsync(ct);
    }
}
