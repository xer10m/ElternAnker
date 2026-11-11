using ElternAnker.Api.Features.Ping;
using ElternAnker.Api.Features.Sessions.Chat;
using ElternAnker.Api.Features.Sessions.Feedback;
using ElternAnker.Api.Features.Sessions.StartSession;
using ElternAnker.Infrastructure.Abstractions;
using ElternAnker.Infrastructure.Extensions;

namespace ElternAnker.Api.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElternAnkerServices(this IServiceCollection services)
    {
        // Infrastructure: AI, VectorStore, State Machine, Storage
        services.AddInfrastructureServices();

        // Feature Handlers (Api-layer only)
        services.AddScoped<IQueryHandler<PingQuery, PingResponse>, PingHandler>();
        services.AddScoped<ICommandHandler<StartSessionCommand, StartSessionResponse>, StartSessionHandler>();
        services.AddScoped<ICommandHandler<ChatCommand, ChatResult>, ChatHandler>();
        services.AddScoped<ICommandHandler<FeedbackCommand, FeedbackResult>, FeedbackHandler>();

        return services;
    }
}
