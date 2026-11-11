using ElternAnker.Infrastructure.Domain;

namespace ElternAnker.Infrastructure.Abstractions;

public interface IRagEngine
{
    public Task<string> GenerateImpulseAsync(
        ReflectionSession session,
        List<PedagogyChunk> contextChunks,
        CancellationToken cancellationToken = default);
}

