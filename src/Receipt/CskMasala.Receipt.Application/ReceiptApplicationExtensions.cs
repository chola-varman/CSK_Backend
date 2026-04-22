using CskMasala.Receipt.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Receipt.Application;

public static class ReceiptApplicationExtensions
{
    public static IServiceCollection AddReceiptModule(this IServiceCollection services)
    {
        services.AddScoped<IReceiptService, ReceiptService>();
        return services;
    }
}
