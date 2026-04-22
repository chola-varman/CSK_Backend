using CskMasala.Shared.Constants;
using Microsoft.AspNetCore.Http;

namespace CskMasala.Shared.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(AppConstants.CorrelationIdHeader, out var correlationId) ||
            string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Items[AppConstants.CorrelationIdHeader] = correlationId.ToString();
        context.Response.Headers[AppConstants.CorrelationIdHeader] = correlationId.ToString();

        await next(context);
    }
}
