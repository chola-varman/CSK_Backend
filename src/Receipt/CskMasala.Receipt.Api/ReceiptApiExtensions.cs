using CskMasala.Receipt.Api.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Receipt.Api;

public static class ReceiptApiExtensions
{
    public static IMvcBuilder AddReceiptControllers(this IMvcBuilder mvc) =>
        mvc.AddApplicationPart(typeof(ReceiptsController).Assembly);
}
