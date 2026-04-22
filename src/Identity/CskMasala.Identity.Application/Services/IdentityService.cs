using CskMasala.Identity.Application.Mappings;
using CskMasala.Identity.Contracts;
using CskMasala.Identity.Domain;
using CskMasala.Shared.Repositories;

namespace CskMasala.Identity.Application.Services;

public class IdentityService(IReadRepository<AppUser> userRead) : IIdentityService
{
    public async Task<UserDto?> GetUserByFirebaseUidAsync(string firebaseUid, CancellationToken ct = default)
    {
        var users = await userRead.FindAsync(u => u.FirebaseUid == firebaseUid, ct);
        return users.FirstOrDefault()?.ToDto();
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await userRead.GetByIdAsync(id, ct);
        return user?.ToDto();
    }
}
