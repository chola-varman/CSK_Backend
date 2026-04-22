using CskMasala.Identity.Application.Commands;
using CskMasala.Identity.Application.Services;
using CskMasala.Identity.Application.Validators;
using CskMasala.Identity.Contracts;
using CskMasala.Shared.Auth;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Identity.Application;

public static class IdentityApplicationExtensions
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddScoped<IExecutionContext, AppExecutionContext>();
        services.AddScoped<IIdentityService, IdentityService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly));
        services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);

        return services;
    }
}
