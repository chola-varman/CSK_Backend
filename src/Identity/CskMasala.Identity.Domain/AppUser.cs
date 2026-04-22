using CskMasala.Shared.Constants;
using CskMasala.Shared.Entities;

namespace CskMasala.Identity.Domain;

public class AppUser : BaseEntity
{
    public string FirebaseUid { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    public Address? DefaultAddress { get; set; }
}
