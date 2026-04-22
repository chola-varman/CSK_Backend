using CskMasala.Identity.Contracts;
using CskMasala.Shared.Auth;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;

namespace CskMasala.Identity.Application;

public class FirebaseAuthMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IExecutionContext executionContext, IIdentityService identityService)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader?.StartsWith("Bearer ") == true)
        {
            var token = authHeader["Bearer ".Length..].Trim();
            try
            {
                var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                var user = await identityService.GetUserByFirebaseUidAsync(decoded.Uid);
                if (user != null && executionContext is AppExecutionContext ctx)
                {
                    ctx.UserId = user.Id;
                    ctx.FirebaseUid = user.FirebaseUid;
                    ctx.Email = user.Email;
                    ctx.PhoneNumber = user.PhoneNumber;
                    ctx.Role = user.Role;
                    ctx.IsAuthenticated = true;
                }
            }
            catch { /* invalid token — continue as guest */ }
        }

        await next(context);
    }
}
