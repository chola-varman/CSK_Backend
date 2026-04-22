using CskMasala.Identity.Api.Controllers;
using CskMasala.Identity.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Identity.Api;

public static class IdentityApiExtensions
{
    public static IMvcBuilder AddIdentityControllers(this IMvcBuilder mvc) =>
        mvc.AddApplicationPart(typeof(AuthController).Assembly);

    public static IApplicationBuilder UseFirebaseAuth(this IApplicationBuilder app) =>
        app.UseMiddleware<FirebaseAuthMiddleware>();
}
