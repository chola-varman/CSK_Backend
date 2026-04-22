using CskMasala.Retail.Application.Commands;
using CskMasala.Retail.Application.Services;
using CskMasala.Retail.Application.Validators;
using CskMasala.Retail.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Retail.Application;

public static class RetailApplicationExtensions
{
    public static IServiceCollection AddRetailApplication(this IServiceCollection services)
    {
        services.AddScoped<IRetailService, RetailService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<IOrderService, OrderService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PlaceOrderCommandHandler).Assembly));
        services.AddValidatorsFromAssembly(typeof(PlaceOrderCommandValidator).Assembly);

        return services;
    }
}
