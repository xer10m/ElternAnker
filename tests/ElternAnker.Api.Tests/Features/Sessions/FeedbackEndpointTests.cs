using System.Net;
using System.Net.Http.Json;
using ElternAnker.Api.Features.Sessions.Feedback;
using ElternAnker.Api.Features.Sessions.StartSession;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace ElternAnker.Api.Tests.Features.Sessions;

public sealed class FeedbackEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_Feedback_WithValidSession_ReturnsOkAndFinalizesState()
    {
        // Arrange
        HttpResponseMessage startResponse = await _client.PostAsJsonAsync("/api/sessions/start", new StartSessionRequest(null, null));
        StartSessionResponse? startBody = await startResponse.Content.ReadFromJsonAsync<StartSessionResponse>();
        startBody.ShouldNotBeNull();

        FeedbackRequest request = new(Rating: 5, Comment: "Der Impuls zum Zähneputzen hat mir sehr geholfen.");

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/sessions/{startBody.SessionId}/feedback", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        FeedbackResponse? body = await response.Content.ReadFromJsonAsync<FeedbackResponse>();
        body.ShouldNotBeNull();
        body.SessionId.ShouldBe(startBody.SessionId);
        body.Message.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Post_Feedback_WithUnknownSession_ReturnsNotFound()
    {
        // Arrange
        Guid unknownSessionId = Guid.NewGuid();
        FeedbackRequest request = new(Rating: 3, Comment: "Test");

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/sessions/{unknownSessionId}/feedback", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
