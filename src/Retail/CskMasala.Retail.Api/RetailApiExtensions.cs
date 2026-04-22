using CskMasala.Retail.Api.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace CskMasala.Retail.Api;

public static class RetailApiExtensions
{
    public static IMvcBuilder AddRetailControllers(this IMvcBuilder mvc) =>
        mvc.AddApplicationPart(typeof(ProductsController).Assembly);
}
