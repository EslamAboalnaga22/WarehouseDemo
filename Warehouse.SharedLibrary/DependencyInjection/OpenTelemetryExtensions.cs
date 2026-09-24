using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Warehouse.SharedLibrary.DependencyInjection
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddObservability(this IServiceCollection services, ILoggingBuilder logging, string serviceName)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddService(serviceName: serviceName, serviceVersion: "1.0.0"))
                .WithTracing(tracing => tracing
                    .AddSource(serviceName)
                    .SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint =
                            new Uri("http://localhost:4317");
                    }))
                .WithMetrics(metrics => metrics
                    .AddMeter(serviceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint =
                            new Uri("http://localhost:4317");
                    }));

            logging.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: "1.0.0"));
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                options.ParseStateValues = true;
                options.AddOtlpExporter(exporterOptions =>
                {
                    exporterOptions.Endpoint =
                        new Uri("http://localhost:4317");
                });
            });

            return services;
        }
    }
}
