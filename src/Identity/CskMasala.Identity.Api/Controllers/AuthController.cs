using CskMasala.Identity.Application.Commands;
using CskMasala.Identity.Application.Queries;
using CskMasala.Shared.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CskMasala.Identity.Api.Controllers;

/// <summary>Authentication and user profile management</summary>
[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator, IExecutionContext ctx) : ControllerBase
{
    /// <summary>Register a new user using a Firebase ID token</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new RegisterUserCommand(request.FirebaseIdToken, request.FullName), ct);
        return Ok(result);
    }

    /// <summary>Get current authenticated user's profile</summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        if (!ctx.IsAuthenticated) return Unauthorized();
        var result = await mediator.Send(new GetUserProfileQuery(ctx.UserId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Update current user's profile</summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        if (!ctx.IsAuthenticated) return Unauthorized();
        var cmd = new UpdateProfileCommand(
            ctx.UserId,
            request.FullName,
            request.PhoneNumber,
            request.Address is null ? null : new AddressDto(
                request.Address.Line1,
                request.Address.Line2,
                request.Address.City,
                request.Address.State,
                request.Address.PinCode));
        var result = await mediator.Send(cmd, ct);
        return Ok(result);
    }
}

public record RegisterRequest(string FirebaseIdToken, string FullName);
public record UpdateProfileRequest(string FullName, string? PhoneNumber, AddressRequest? Address);
public record AddressRequest(string Line1, string? Line2, string City, string State, string PinCode);
