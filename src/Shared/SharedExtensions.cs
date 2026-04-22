using CskMasala.Shared.Behaviours;
using CskMasala.Shared.Middleware;
using CskMasala.Shared.Redis;
using CskMasala.Shared.Telemetry;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Shared;

public static class SharedExtensions
{
    public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration config)
    {
        var redisConn = config["REDIS_CONNECTION"] ?? "localhost:6379";
        services.AddStackExchangeRedisCache(o => o.Configuration = redisConn);
        services.AddSingleton<IRedisCache, RedisCache>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        services.AddCskTelemetry(config);

        return services;
    }

    public static IApplicationBuilder UseSharedMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}
