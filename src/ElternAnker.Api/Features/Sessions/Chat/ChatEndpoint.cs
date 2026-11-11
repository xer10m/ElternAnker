using System.Globalization;
using ElternAnker.Infrastructure.Abstractions;
using ElternAnker.Infrastructure.Domain;

namespace ElternAnker.Api.Features.Sessions.Chat;

public static class ChatEndpoint
{
    public static RouteGroupBuilder MapChatEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/chat", async (
            Guid id,
            ChatRequest request,
            ICommandHandler<ChatCommand, ChatResult> handler,
            CancellationToken ct) =>
        {
            ChatCommand command = new(id, request.Message);
            ChatResult result = await handler.HandleAsync(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Response)
                : Results.NotFound(new { message = result.ErrorMessage });
        })
        .WithName("ChatSession")
        .WithSummary("Processes a conversational turn in a reflection session")
        .Produces<ChatResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}

public sealed record ChatRequest(string Message);

public sealed record ChatCommand(Guid SessionId, string Message);

public sealed record ChatResponse(
    Guid SessionId,
    SessionState State,
    string AssistantMessage,
    DateTimeOffset Timestamp);

public sealed record ChatResult(bool IsSuccess, ChatResponse? Response, string? ErrorMessage = null)
{
    public static ChatResult Success(ChatResponse response) => new(true, response);
    public static ChatResult NotFound(string message) => new(false, null, message);
}

public sealed class ChatHandler(
    ISessionStore sessionStore,
    IDialogStateMachine stateMachine) : ICommandHandler<ChatCommand, ChatResult>
{
    private readonly ISessionStore _sessionStore = sessionStore;
    private readonly IDialogStateMachine _stateMachine = stateMachine;

    public async Task<ChatResult> HandleAsync(
        ChatCommand command,
        CancellationToken cancellationToken = default)
    {
        ReflectionSession? session = await _sessionStore.GetSessionAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            return ChatResult.NotFound(string.Create(CultureInfo.InvariantCulture, $"Session with ID '{command.SessionId}' was not found."));
        }

        DialogTurnResult turnResult = await _stateMachine.ProcessTurnAsync(session, command.Message, cancellationToken);
        await _sessionStore.SaveSessionAsync(turnResult.Session, cancellationToken);

        ChatResponse response = new(
            SessionId: turnResult.Session.SessionId,
            State: turnResult.Session.CurrentState,
            AssistantMessage: turnResult.AssistantMessage,
            Timestamp: DateTimeOffset.UtcNow);

        return ChatResult.Success(response);
    }
}
