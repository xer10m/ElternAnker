using System.Net;
using System.Net.Http.Json;
using ElternAnker.Api.Features.Ping;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace ElternAnker.Api.Tests.Features.Ping;

public sealed class PingEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_Ping_ReturnsOkWithPongMessage()
    {
        // Act
        var response = await _client.GetAsync("/ping");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PingResponse>();
        body.ShouldNotBeNull();
        body.Message.ShouldBe("pong");
        body.Timestamp.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }
}
