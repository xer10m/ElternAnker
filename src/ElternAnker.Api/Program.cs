using ElternAnker.Api.Features.Health;
using ElternAnker.Api.Features.Ping;
using ElternAnker.Api.Infrastructure.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Aspire Service Defaults: OpenTelemetry, Discovery & default Health Checks (/health, /alive)
builder.AddServiceDefaults();

// OpenAPI support for development
builder.Services.AddOpenApi();

builder.Services.AddElternAnkerServices();

var app = builder.Build();

// Aspire default endpoints (/health, /alive)
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Feature Endpoints
app.MapPingEndpoints();
app.MapHealthEndpoints();

await app.RunAsync();

// Required for WebApplicationFactory in integration tests
public partial class Program { }
