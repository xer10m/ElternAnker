using ElternAnker.Infrastructure.Domain;

namespace ElternAnker.Infrastructure.Abstractions;

public sealed record DialogTurnResult(
    ReflectionSession Session,
    string AssistantMessage);

public interface IDialogStateMachine
{
    public Task<DialogTurnResult> ProcessTurnAsync(
        ReflectionSession session,
        string userMessage,
        CancellationToken cancellationToken = default);
}
