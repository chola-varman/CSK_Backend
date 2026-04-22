using CskMasala.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CskMasala.Shared.Telemetry;

public static class TelemetryExtensions
{
    public static IServiceCollection AddCskTelemetry(this IServiceCollection services, IConfiguration config)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(AppConstants.ServiceName, serviceVersion: AppConstants.ServiceVersion))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                // Local OTLP collector (e.g. Jaeger with OTLP receiver, or any collector)
                var localOtlp = config["OTLP_ENDPOINT"];
                if (!string.IsNullOrEmpty(localOtlp))
                {
                    builder.AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(localOtlp);
                        o.Protocol = OtlpExportProtocol.Grpc;
                    });
                }

                // Grafana Cloud Tempo
                var grafanaOtlp = config["GRAFANA_CLOUD_TEMPO_URL"];
                var grafanaKey = config["GRAFANA_CLOUD_API_KEY"];
                if (!string.IsNullOrEmpty(grafanaOtlp) && !string.IsNullOrEmpty(grafanaKey))
                {
                    builder.AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(grafanaOtlp);
                        o.Protocol = OtlpExportProtocol.HttpProtobuf;
                        o.Headers = $"Authorization=Basic {grafanaKey}";
                    });
                }
            });

        return services;
    }
}
