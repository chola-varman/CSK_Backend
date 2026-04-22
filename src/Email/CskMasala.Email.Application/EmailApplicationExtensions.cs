using CskMasala.Email.Application.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Email.Application;

public static class EmailApplicationExtensions
{
    public static IServiceCollection AddEmailApplication(this IServiceCollection services)
    {
        services.AddSingleton<EmailQueue>();
        services.AddTransient<SmtpEmailSender>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrderPlacedEmailHandler).Assembly));
        return services;
    }
}
