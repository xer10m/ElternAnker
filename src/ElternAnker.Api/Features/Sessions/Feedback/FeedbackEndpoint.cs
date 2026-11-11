using System.Globalization;
using ElternAnker.Infrastructure.Abstractions;
using ElternAnker.Infrastructure.Domain;

namespace ElternAnker.Api.Features.Sessions.Feedback;

public static class FeedbackEndpoint
{
    public static RouteGroupBuilder MapFeedbackEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/feedback", async (
            Guid id,
            FeedbackRequest request,
            ICommandHandler<FeedbackCommand, FeedbackResult> handler,
            CancellationToken ct) =>
        {
            FeedbackCommand command = new(id, request.Rating, request.Comment);
            FeedbackResult result = await handler.HandleAsync(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Response)
                : Results.NotFound(new { message = result.ErrorMessage });
        })
        .WithName("SubmitFeedback")
        .WithSummary("Records user feedback for a completed reflection session")
        .Produces<FeedbackResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}

public sealed record FeedbackRequest(int Rating, string? Comment);

public sealed record FeedbackCommand(Guid SessionId, int Rating, string? Comment);

public sealed record FeedbackResponse(Guid SessionId, string Message);

public sealed record FeedbackResult(bool IsSuccess, FeedbackResponse? Response, string? ErrorMessage = null)
{
    public static FeedbackResult Success(FeedbackResponse response) => new(true, response);
    public static FeedbackResult NotFound(string message) => new(false, null, message);
}

public sealed class FeedbackHandler(ISessionStore sessionStore)
    : ICommandHandler<FeedbackCommand, FeedbackResult>
{
    private readonly ISessionStore _sessionStore = sessionStore;

    public async Task<FeedbackResult> HandleAsync(
        FeedbackCommand command,
        CancellationToken cancellationToken = default)
    {
        ReflectionSession? session = await _sessionStore.GetSessionAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            return FeedbackResult.NotFound(string.Create(CultureInfo.InvariantCulture, $"Session with ID '{command.SessionId}' was not found."));
        }

        session.ConversationHistory.Add(new ChatMessage(
            ChatRoles.System,
            string.Create(CultureInfo.InvariantCulture, $"Feedback received: Rating {command.Rating}/5, Comment: {command.Comment ?? "None"}"),
            DateTimeOffset.UtcNow));

        ReflectionSession updatedSession = session with { CurrentState = SessionState.AwaitingFeedback };
        await _sessionStore.SaveSessionAsync(updatedSession, cancellationToken);

        FeedbackResponse response = new(
            SessionId: session.SessionId,
            Message: "Vielen Dank für dein Feedback! Deine Rückmeldung hilft uns, ElternAnker stetig zu verbessern.");

        return FeedbackResult.Success(response);
    }
}
