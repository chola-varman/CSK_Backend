using CskMasala.Identity.Application.Mappings;
using CskMasala.Identity.Contracts;
using CskMasala.Identity.Domain;
using CskMasala.Shared.Repositories;
using FirebaseAdmin.Auth;
using MediatR;

namespace CskMasala.Identity.Application.Commands;

public record RegisterUserCommand(string FirebaseIdToken, string FullName) : IRequest<UserDto>;

public class RegisterUserCommandHandler(
    IWriteRepository<AppUser> userWrite,
    IReadRepository<AppUser> userRead)
    : IRequestHandler<RegisterUserCommand, UserDto>
{
    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.FirebaseIdToken, ct);

        var existing = await userRead.FindAsync(u => u.FirebaseUid == decoded.Uid, ct);
        if (existing.Any())
            return existing.First().ToDto();

        var user = new AppUser
        {
            FirebaseUid = decoded.Uid,
            Email = decoded.Claims.TryGetValue("email", out var email) ? email.ToString()! : string.Empty,
            PhoneNumber = decoded.Claims.TryGetValue("phone_number", out var phone) ? phone.ToString() : null,
            FullName = request.FullName
        };

        await userWrite.AddAsync(user, ct);
        await userWrite.SaveChangesAsync(ct);
        return user.ToDto();
    }
}
