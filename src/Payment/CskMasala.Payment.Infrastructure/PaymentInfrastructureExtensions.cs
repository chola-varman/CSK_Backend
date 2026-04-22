using CskMasala.Payment.Application;
using CskMasala.Payment.Infrastructure.Persistence;
using CskMasala.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Payment.Infrastructure;

public static class PaymentInfrastructureExtensions
{
    public static IServiceCollection AddPaymentModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<PaymentDbContext>(opts =>
            opts.UseNpgsql(config["CONN_PAYMENT"]));

        services.AddScoped<IReadRepository<Domain.Payment>>(sp =>
            new ReadRepository<Domain.Payment>(sp.GetRequiredService<PaymentDbContext>()));
        services.AddScoped<IWriteRepository<Domain.Payment>>(sp =>
            new WriteRepository<Domain.Payment>(sp.GetRequiredService<PaymentDbContext>()));

        services.AddPaymentApplication();

        return services;
    }
}
