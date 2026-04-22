using CskMasala.Retail.Application;
using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Retail.Infrastructure.Persistence;
using CskMasala.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Retail.Infrastructure;

public static class RetailInfrastructureExtensions
{
    public static IServiceCollection AddRetailModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<RetailDbContext>(opts =>
            opts.UseNpgsql(config["CONN_RETAIL"]));

        services.AddScoped<IReadRepository<Category>>(sp =>
            new ReadRepository<Category>(sp.GetRequiredService<RetailDbContext>()));
        services.AddScoped<IWriteRepository<Category>>(sp =>
            new WriteRepository<Category>(sp.GetRequiredService<RetailDbContext>()));

        services.AddScoped<IReadRepository<Product>>(sp =>
            new ReadRepository<Product>(sp.GetRequiredService<RetailDbContext>()));
        services.AddScoped<IWriteRepository<Product>>(sp =>
            new WriteRepository<Product>(sp.GetRequiredService<RetailDbContext>()));

        services.AddScoped<IReadRepository<Coupon>>(sp =>
            new ReadRepository<Coupon>(sp.GetRequiredService<RetailDbContext>()));
        services.AddScoped<IWriteRepository<Coupon>>(sp =>
            new WriteRepository<Coupon>(sp.GetRequiredService<RetailDbContext>()));

        services.AddScoped<IReadRepository<ShippingRate>>(sp =>
            new ReadRepository<ShippingRate>(sp.GetRequiredService<RetailDbContext>()));

        services.AddScoped<IReadRepository<Order>>(sp =>
            new ReadRepository<Order>(sp.GetRequiredService<RetailDbContext>()));
        services.AddScoped<IWriteRepository<Order>>(sp =>
            new WriteRepository<Order>(sp.GetRequiredService<RetailDbContext>()));

        services.AddRetailApplication();

        return services;
    }
}
