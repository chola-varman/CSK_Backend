using CskMasala.Identity.Contracts;
using CskMasala.Identity.Domain;

namespace CskMasala.Identity.Application.Mappings;

internal static class UserMappings
{
    internal static UserDto ToDto(this AppUser user) => new(
        user.Id,
        user.FirebaseUid,
        user.Email,
        user.PhoneNumber,
        user.FullName,
        user.Role);
}
