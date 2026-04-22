using CskMasala.Identity.Application.Mappings;
using CskMasala.Identity.Contracts;
using CskMasala.Identity.Domain;
using CskMasala.Shared.Repositories;
using MediatR;

namespace CskMasala.Identity.Application.Commands;

public record AddressDto(string Line1, string? Line2, string City, string State, string PinCode);
public record UpdateProfileCommand(Guid UserId, string FullName, string? PhoneNumber, AddressDto? Address) : IRequest<UserDto>;

public class UpdateProfileCommandHandler(
    IReadRepository<AppUser> userRead,
    IWriteRepository<AppUser> userWrite)
    : IRequestHandler<UpdateProfileCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var user = await userRead.GetByIdAsync(request.UserId, ct)
            ?? throw new KeyNotFoundException("User not found");

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.UpdatedAt = DateTime.UtcNow;

        if (request.Address != null)
        {
            user.DefaultAddress = new Address
            {
                Line1 = request.Address.Line1,
                Line2 = request.Address.Line2,
                City = request.Address.City,
                State = request.Address.State,
                PinCode = request.Address.PinCode
            };
        }

        userWrite.Update(user);
        await userWrite.SaveChangesAsync(ct);
        return user.ToDto();
    }
}
