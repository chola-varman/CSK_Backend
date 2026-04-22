using CskMasala.Identity.Application.Mappings;
using CskMasala.Identity.Contracts;
using CskMasala.Identity.Domain;
using CskMasala.Shared.Repositories;
using MediatR;

namespace CskMasala.Identity.Application.Queries;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserDto?>;

public class GetUserProfileQueryHandler(IReadRepository<AppUser> userRead)
    : IRequestHandler<GetUserProfileQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserProfileQuery request, CancellationToken ct)
    {
        var user = await userRead.GetByIdAsync(request.UserId, ct);
        return user?.ToDto();
    }
}
