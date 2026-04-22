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
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly));
        services.AddValidatorsFromAssembly(typeof(CreateProductCommandValidator).Assembly);
        return services;
    }
}
