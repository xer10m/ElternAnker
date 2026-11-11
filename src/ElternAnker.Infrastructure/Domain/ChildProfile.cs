namespace ElternAnker.Infrastructure.Domain;

public sealed record ChildProfile(
    Guid Id,
    string Name,
    int Age,
    IReadOnlyList<string> KnownTraits,
    IReadOnlyList<string> History);

