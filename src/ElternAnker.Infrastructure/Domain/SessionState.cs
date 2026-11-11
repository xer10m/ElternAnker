namespace ElternAnker.Infrastructure.Domain;

public enum SessionState
{
    CollectingContext,
    HypothesisFallback,
    DeliveringImpulse,
    AwaitingFeedback
}

