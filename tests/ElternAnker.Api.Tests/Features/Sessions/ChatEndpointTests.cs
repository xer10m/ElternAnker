using System.Net;
using System.Net.Http.Json;
using ElternAnker.Api.Features.Sessions.Chat;
using ElternAnker.Api.Features.Sessions.StartSession;
using ElternAnker.Infrastructure.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace ElternAnker.Api.Tests.Features.Sessions;

public sealed class ChatEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_Chat_WithValidSession_ReturnsOkAndTransitionsState()
    {
        // Arrange - start session
        HttpResponseMessage startResponse = await _client.PostAsJsonAsync("/api/sessions/start", new StartSessionRequest(null, null));
        startResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        StartSessionResponse? startBody = await startResponse.Content.ReadFromJsonAsync<StartSessionResponse>();
        startBody.ShouldNotBeNull();

        ChatRequest chatRequest = new("Es gab riesiges Geschrei beim Zähneputzen.");

        // Act
        HttpResponseMessage chatResponse = await _client.PostAsJsonAsync($"/api/sessions/{startBody.SessionId}/chat", chatRequest);

        // Assert
        chatResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        ChatResponse? chatBody = await chatResponse.Content.ReadFromJsonAsync<ChatResponse>();
        chatBody.ShouldNotBeNull();
        chatBody.SessionId.ShouldBe(startBody.SessionId);
        chatBody.State.ShouldBe(SessionState.HypothesisFallback);
        chatBody.AssistantMessage.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Post_Chat_WithUnknownSession_ReturnsNotFound()
    {
        // Arrange
        Guid unknownSessionId = Guid.NewGuid();
        ChatRequest chatRequest = new("Hallo?");

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/sessions/{unknownSessionId}/chat", chatRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
