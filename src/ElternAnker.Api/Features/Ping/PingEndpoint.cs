using ElternAnker.Api.Infrastructure.Abstractions;

namespace ElternAnker.Api.Features.Ping;

public static class PingEndpoint
{
    public static IEndpointRouteBuilder MapPingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/ping", async (IQueryHandler<PingQuery, PingResponse> handler, CancellationToken ct) =>
        {
            var response = await handler.HandleAsync(new PingQuery(), ct);
            return Results.Ok(response);
        })
        .WithName("Ping")
        .WithTags("Health")
        .Produces<PingResponse>();

        return app;
    }
}

public sealed record PingQuery;

public sealed record PingResponse(string Message, DateTimeOffset Timestamp);

public sealed class PingHandler : IQueryHandler<PingQuery, PingResponse>
{
    public Task<PingResponse> HandleAsync(PingQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new PingResponse("pong", DateTimeOffset.UtcNow));
    }
}
