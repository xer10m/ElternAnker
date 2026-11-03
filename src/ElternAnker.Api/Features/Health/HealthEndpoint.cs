using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ElternAnker.Api.Features.Health;

public static class HealthEndpoint
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health/detail", new HealthCheckOptions
        {
            ResponseWriter = WriteJsonResponse
        })
        .WithName("HealthDetail")
        .WithTags("Health");

        return app;
    }

    private static Task WriteJsonResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration,
            entries = report.Entries.ToDictionary(
                e => e.Key,
                e => new { status = e.Value.Status.ToString(), duration = e.Value.Duration })
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
}

