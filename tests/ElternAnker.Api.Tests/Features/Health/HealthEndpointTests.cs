using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace ElternAnker.Api.Tests.Features.Health;

public sealed class HealthEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_Health_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_HealthDetail_ReturnsJsonWithStatus()
    {
        // Act
        var response = await _client.GetAsync("/health/detail");
        string content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        content.ShouldContain("\"status\"");
        content.ShouldContain("Healthy");
    }
}
