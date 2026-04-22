namespace CskMasala.Identity.Contracts;

public interface IIdentityService
{
    Task<UserDto?> GetUserByFirebaseUidAsync(string firebaseUid, CancellationToken ct = default);
    Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken ct = default);
}
