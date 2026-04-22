using CskMasala.Shared.Auth;
using CskMasala.Shared.Constants;

namespace CskMasala.Identity.Application;

public class AppExecutionContext : IExecutionContext
{
    public Guid UserId { get; set; }
    public string FirebaseUid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; }
    public bool IsAuthenticated { get; set; }
}
