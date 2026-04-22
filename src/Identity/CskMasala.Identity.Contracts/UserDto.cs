using CskMasala.Shared.Constants;

namespace CskMasala.Identity.Contracts;

public record UserDto(
    Guid Id,
    string FirebaseUid,
    string Email,
    string? PhoneNumber,
    string FullName,
    UserRole Role);
