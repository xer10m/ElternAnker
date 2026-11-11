using System.Net;
using System.Net.Http.Json;
using ElternAnker.Api.Features.Sessions.StartSession;
using ElternAnker.Infrastructure.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace ElternAnker.Api.Tests.Features.Sessions;

public sealed class StartSessionEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_StartSession_ReturnsOkWithCollectingContextState()
    {
        // Arrange
        StartSessionRequest request = new(ChildId: null, InitialTopic: "Abendunruhe");

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/sessions/start", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        StartSessionResponse? body = await response.Content.ReadFromJsonAsync<StartSessionResponse>();
        body.ShouldNotBeNull();
        body.SessionId.ShouldNotBe(Guid.Empty);
        body.ChildId.ShouldNotBe(Guid.Empty);
        body.State.ShouldBe(SessionState.CollectingContext);
        body.WelcomeMessage.ShouldNotBeNullOrWhiteSpace();
    }
}
