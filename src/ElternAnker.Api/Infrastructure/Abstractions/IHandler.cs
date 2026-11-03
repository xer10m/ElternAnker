namespace ElternAnker.Api.Infrastructure.Abstractions;

/// <summary>
/// Lightweight handler contract without third-party dependencies (Vertical Slice Architecture).
/// </summary>
public interface ICommandHandler<in TCommand, TResult>
{
    public Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Lightweight query handler contract without third-party dependencies (Vertical Slice Architecture).
/// </summary>
public interface IQueryHandler<in TQuery, TResult>
{
    public Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}

