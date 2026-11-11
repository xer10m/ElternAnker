using ElternAnker.Infrastructure.Domain;

namespace ElternAnker.Infrastructure.Abstractions;

public interface ISessionStore
{
    public Task<ReflectionSession?> GetSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    public Task SaveSessionAsync(ReflectionSession session, CancellationToken cancellationToken = default);
}
