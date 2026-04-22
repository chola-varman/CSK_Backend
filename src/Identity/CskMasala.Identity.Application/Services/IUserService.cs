using CskMasala.Identity.Contracts;
using CskMasala.Identity.Domain;

namespace CskMasala.Identity.Application.Services;

public record UserAddressInput(string Line1, string? Line2, string City, string State, string PinCode);

public interface IUserService
{
    Task<UserDto> RegisterAsync(string firebaseIdToken, string fullName, CancellationToken ct = default);
    Task<UserDto?> GetProfileAsync(Guid userId, CancellationToken ct = default);
    Task<UserDto> UpdateProfileAsync(Guid userId, string fullName, string? phoneNumber, UserAddressInput? address, CancellationToken ct = default);
}
