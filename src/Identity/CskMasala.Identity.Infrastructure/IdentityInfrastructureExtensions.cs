using CskMasala.Identity.Application;
using CskMasala.Identity.Domain;
using CskMasala.Identity.Infrastructure.Persistence;
using CskMasala.Shared.Repositories;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Identity.Infrastructure;

public static class IdentityInfrastructureExtensions
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<IdentityDbContext>(opts =>
            opts.UseNpgsql(config["CONN_IDENTITY"]));

        services.AddScoped<IReadRepository<AppUser>>(sp =>
            new ReadRepository<AppUser>(sp.GetRequiredService<IdentityDbContext>()));
        services.AddScoped<IWriteRepository<AppUser>>(sp =>
            new WriteRepository<AppUser>(sp.GetRequiredService<IdentityDbContext>()));

        services.AddIdentityApplication();

        InitializeFirebase(config);

        return services;
    }

    private static void InitializeFirebase(IConfiguration config)
    {
        if (FirebaseApp.DefaultInstance != null) return;

        var json = config["FIREBASE_SERVICE_ACCOUNT_JSON"];
        if (!string.IsNullOrWhiteSpace(json))
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(json),
                ProjectId = config["FIREBASE_PROJECT_ID"]
            });
        }
        else
        {
            FirebaseApp.Create(new AppOptions
            {
                ProjectId = config["FIREBASE_PROJECT_ID"] ?? "demo-project"
            });
        }
    }
}
