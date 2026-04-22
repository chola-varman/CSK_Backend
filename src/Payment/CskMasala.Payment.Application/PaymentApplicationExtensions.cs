using CskMasala.Payment.Application.Commands;
using CskMasala.Payment.Application.Services;
using CskMasala.Payment.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Payment.Application;

public static class PaymentApplicationExtensions
{
    public static IServiceCollection AddPaymentApplication(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddHttpClient<RazorpayClient>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateRazorpayOrderCommandHandler).Assembly));
        services.AddValidatorsFromAssembly(typeof(CreateRazorpayOrderCommandHandler).Assembly);
        return services;
    }
}
