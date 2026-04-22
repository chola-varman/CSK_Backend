using CskMasala.Identity.Application.Mappings;
using CskMasala.Identity.Contracts;
using CskMasala.Identity.Domain;
using CskMasala.Shared.Repositories;
using FirebaseAdmin.Auth;

namespace CskMasala.Identity.Application.Services;

public class UserService(
    IReadRepository<AppUser> userRead,
    IWriteRepository<AppUser> userWrite) : IUserService
{
    public async Task<UserDto> RegisterAsync(string firebaseIdToken, string fullName, CancellationToken ct = default)
    {
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseIdToken, ct);

        var existing = await userRead.FindAsync(u => u.FirebaseUid == decoded.Uid, ct);
        if (existing.Any())
            return existing.First().ToDto();

        var user = new AppUser
        {
            FirebaseUid = decoded.Uid,
            Email = decoded.Claims.TryGetValue("email", out var email) ? email.ToString()! : string.Empty,
            PhoneNumber = decoded.Claims.TryGetValue("phone_number", out var phone) ? phone.ToString() : null,
            FullName = fullName
        };

        await userWrite.AddAsync(user, ct);
        await userWrite.SaveChangesAsync(ct);
        return user.ToDto();
    }

    public async Task<UserDto?> GetProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userRead.GetByIdAsync(userId, ct);
        return user?.ToDto();
    }

    public async Task<UserDto> UpdateProfileAsync(Guid userId, string fullName, string? phoneNumber, UserAddressInput? address, CancellationToken ct = default)
    {
        var user = await userRead.GetByIdAsync(userId, ct)
            ?? throw new KeyNotFoundException("User not found");

        user.FullName = fullName;
        user.PhoneNumber = phoneNumber;
        user.UpdatedAt = DateTime.UtcNow;

        if (address != null)
        {
            user.DefaultAddress = new Address
            {
                Line1 = address.Line1,
                Line2 = address.Line2,
                City = address.City,
                State = address.State,
                PinCode = address.PinCode
            };
        }

        userWrite.Update(user);
        await userWrite.SaveChangesAsync(ct);
        return user.ToDto();
    }
}
