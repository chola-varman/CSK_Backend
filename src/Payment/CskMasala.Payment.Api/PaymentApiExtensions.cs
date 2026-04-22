using CskMasala.Payment.Api.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Payment.Api;

public static class PaymentApiExtensions
{
    public static IMvcBuilder AddPaymentControllers(this IMvcBuilder mvc) =>
        mvc.AddApplicationPart(typeof(PaymentsController).Assembly);
}
