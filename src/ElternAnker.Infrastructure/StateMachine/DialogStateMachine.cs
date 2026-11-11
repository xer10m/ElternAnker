using System.Globalization;
using ElternAnker.Infrastructure.Abstractions;
using ElternAnker.Infrastructure.Domain;

namespace ElternAnker.Infrastructure.StateMachine;

public sealed class DialogStateMachine(
    IPedagogyVectorStore vectorStore,
    IRagEngine ragEngine) : IDialogStateMachine
{
    private readonly IPedagogyVectorStore _vectorStore = vectorStore;
    private readonly IRagEngine _ragEngine = ragEngine;

    public async Task<DialogTurnResult> ProcessTurnAsync(
        ReflectionSession session,
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        // 1. Record incoming user turn
        session.ConversationHistory.Add(new ChatMessage(ChatRoles.User, userMessage, DateTimeOffset.UtcNow));

        string assistantMessage;
        SessionState nextState;
        string? selectedImpulse = session.SelectedImpulse;

        // 2. Handle state transitions with single-question guardrail
        switch (session.CurrentState)
        {
            case SessionState.CollectingContext:
                assistantMessage = "Ich höre, wie herausfordernd diese Situation war. Welcher Moment davon belastet dich im Rückblick am meisten?";
                nextState = SessionState.HypothesisFallback;
                break;

            case SessionState.HypothesisFallback:
                assistantMessage = "Oft suchen Kinder nach einem anstrengenden Tag auf diese Weise wieder Halt und Bindung. Klingt das für dich stimmig?";
                nextState = SessionState.DeliveringImpulse;
                break;

            case SessionState.DeliveringImpulse:
                IReadOnlyList<PedagogyChunk> chunks = await _vectorStore.SearchAsync(userMessage, ageYears: 4, limit: 2, cancellationToken);
                string impulse = await _ragEngine.GenerateImpulseAsync(session, chunks.ToList(), cancellationToken);
                selectedImpulse = impulse;
                assistantMessage = string.Create(CultureInfo.InvariantCulture, $"{impulse}\n\nWie klingt dieser Gedanke für deinen Alltag?");
                nextState = SessionState.AwaitingFeedback;
                break;

            case SessionState.AwaitingFeedback:
                assistantMessage = "Vielen Dank für deine Reflexion heute Abend. Nimm dir einen Moment für dich – du begleitest dein Kind mit viel Fürsorge.";
                nextState = SessionState.AwaitingFeedback;
                break;

            default:
                assistantMessage = "Wie kann ich dich heute Abend bei deiner Reflexion unterstützen?";
                nextState = SessionState.CollectingContext;
                break;
        }

        // 3. Record assistant message
        session.ConversationHistory.Add(new ChatMessage(ChatRoles.Assistant, assistantMessage, DateTimeOffset.UtcNow));

        ReflectionSession updatedSession = session with
        {
            CurrentState = nextState,
            SelectedImpulse = selectedImpulse
        };

        return new DialogTurnResult(updatedSession, assistantMessage);
    }
}

