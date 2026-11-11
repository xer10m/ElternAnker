using ElternAnker.Infrastructure.Domain;

namespace ElternAnker.Infrastructure.Abstractions;

public interface IPedagogyVectorStore
{
    public Task<IReadOnlyList<PedagogyChunk>> SearchAsync(
        string query,
        int ageYears,
        int limit = 3,
        CancellationToken cancellationToken = default);
}

