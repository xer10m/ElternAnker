using ElternAnker.Infrastructure.Abstractions;
using ElternAnker.Infrastructure.Domain;
using ElternAnker.Infrastructure.Rag;
using ElternAnker.Infrastructure.StateMachine;
using ElternAnker.Infrastructure.VectorStore;
using Shouldly;
using Xunit;

namespace ElternAnker.Api.Tests.Features.Sessions;

public sealed class DialogStateMachineTests
{
    private readonly DialogStateMachine _sut = new(new InMemoryVectorStore(), new StubRagEngine());

    [Fact]
    public async Task ProcessTurnAsync_CyclesThroughStatesProperly()
    {
        // 1. Initial State: CollectingContext
        ReflectionSession session = new(
            SessionId: Guid.NewGuid(),
            ChildId: Guid.NewGuid(),
            CurrentState: SessionState.CollectingContext,
            ConversationHistory: []);

        // Turn 1: CollectingContext -> HypothesisFallback
        DialogTurnResult turn1 = await _sut.ProcessTurnAsync(session, "Mein Kind weinte beim Anziehen.");
        turn1.Session.CurrentState.ShouldBe(SessionState.HypothesisFallback);
        turn1.AssistantMessage.ShouldContain("?");

        // Turn 2: HypothesisFallback -> DeliveringImpulse
        DialogTurnResult turn2 = await _sut.ProcessTurnAsync(turn1.Session, "Ja, das stimmt, es war sehr laut heute.");
        turn2.Session.CurrentState.ShouldBe(SessionState.DeliveringImpulse);
        turn2.AssistantMessage.ShouldContain("?");

        // Turn 3: DeliveringImpulse -> AwaitingFeedback
        DialogTurnResult turn3 = await _sut.ProcessTurnAsync(turn2.Session, "Gerne.");
        turn3.Session.CurrentState.ShouldBe(SessionState.AwaitingFeedback);
        turn3.Session.SelectedImpulse.ShouldNotBeNullOrWhiteSpace();
        turn3.AssistantMessage.ShouldContain("?");

        // Turn 4: AwaitingFeedback stays in AwaitingFeedback
        DialogTurnResult turn4 = await _sut.ProcessTurnAsync(turn3.Session, "Danke, das probiere ich aus.");
        turn4.Session.CurrentState.ShouldBe(SessionState.AwaitingFeedback);
    }
}
