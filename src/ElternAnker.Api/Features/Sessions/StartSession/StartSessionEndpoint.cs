using ElternAnker.Infrastructure.Abstractions;
using ElternAnker.Infrastructure.Domain;

namespace ElternAnker.Api.Features.Sessions.StartSession;

public static class StartSessionEndpoint
{
    public static RouteGroupBuilder MapStartSessionEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/start", async (
            StartSessionRequest request,
            ICommandHandler<StartSessionCommand, StartSessionResponse> handler,
            CancellationToken ct) =>
        {
            StartSessionCommand command = new(request.ChildId, request.InitialTopic);
            StartSessionResponse response = await handler.HandleAsync(command, ct);
            return Results.Ok(response);
        })
        .WithName("StartSession")
        .WithSummary("Starts a new reflection session for a child profile")
        .Produces<StartSessionResponse>();

        return group;
    }
}

public sealed record StartSessionRequest(Guid? ChildId, string? InitialTopic);

public sealed record StartSessionCommand(Guid? ChildId, string? InitialTopic);

public sealed record StartSessionResponse(
    Guid SessionId,
    Guid ChildId,
    SessionState State,
    string WelcomeMessage);

public sealed class StartSessionHandler(ISessionStore sessionStore)
    : ICommandHandler<StartSessionCommand, StartSessionResponse>
{
    private readonly ISessionStore _sessionStore = sessionStore;
    private static readonly Guid _defaultDemoChildId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public async Task<StartSessionResponse> HandleAsync(
        StartSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        Guid childId = command.ChildId ?? _defaultDemoChildId;
        Guid sessionId = Guid.NewGuid();

        const string welcomeMessage = "Willkommen bei deiner Abendlicht-Reflexion. Wie war dein heutiger Tag mit deinem Kind?";

        ReflectionSession session = new(
            SessionId: sessionId,
            ChildId: childId,
            CurrentState: SessionState.CollectingContext,
            ConversationHistory:
            [
                new ChatMessage(ChatRoles.Assistant, welcomeMessage, DateTimeOffset.UtcNow)
            ]);

        await _sessionStore.SaveSessionAsync(session, cancellationToken);

        return new StartSessionResponse(
            SessionId: sessionId,
            ChildId: childId,
            State: SessionState.CollectingContext,
            WelcomeMessage: welcomeMessage);
    }
}
