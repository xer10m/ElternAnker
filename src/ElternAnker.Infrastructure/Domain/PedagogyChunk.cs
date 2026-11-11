namespace ElternAnker.Infrastructure.Domain;

public sealed record PedagogyChunk(
    string Id,
    string Topic,
    string AgeRange,
    string CoreConcept,
    string ReframingPerspective,
    IReadOnlyList<string> ActionableIdeas,
    string ReflectionQuestion);

