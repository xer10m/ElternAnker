using System.Collections.Concurrent;
using ElternAnker.Infrastructure.Abstractions;
using ElternAnker.Infrastructure.Domain;

namespace ElternAnker.Infrastructure.Storage;

public sealed class InMemorySessionStore : ISessionStore
{
    private readonly ConcurrentDictionary<Guid, ReflectionSession> _sessions = new();

    public Task<ReflectionSession?> GetSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        _sessions.TryGetValue(sessionId, out ReflectionSession? session);
        return Task.FromResult(session);
    }

    public Task SaveSessionAsync(ReflectionSession session, CancellationToken cancellationToken = default)
    {
        _sessions[session.SessionId] = session;
        return Task.CompletedTask;
    }
}
