using CskMasala.Email.Application;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Email.Worker;

public static class EmailWorkerExtensions
{
    public static IServiceCollection AddEmailWorker(this IServiceCollection services)
    {
        services.AddEmailApplication();
        services.AddHostedService<EmailBackgroundService>();
        return services;
    }
}
