using CskMasala.Shared.Constants;

namespace CskMasala.Shared.Auth;

public interface IExecutionContext
{
    Guid UserId { get; }
    string FirebaseUid { get; }
    string Email { get; }
    string? PhoneNumber { get; }
    UserRole Role { get; }
    bool IsAuthenticated { get; }
}
