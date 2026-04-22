using CskMasala.Identity.Application.Services;
using CskMasala.Identity.Contracts;
using CskMasala.Shared.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Identity.Application;

public static class IdentityApplicationExtensions
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddScoped<IExecutionContext, AppExecutionContext>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
