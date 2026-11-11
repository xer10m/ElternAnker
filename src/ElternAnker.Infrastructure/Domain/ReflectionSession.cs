namespace ElternAnker.Infrastructure.Domain;

public sealed record ReflectionSession(
    Guid SessionId,
    Guid ChildId,
    SessionState CurrentState,
    List<ChatMessage> ConversationHistory,
    string? SelectedImpulse = null);

