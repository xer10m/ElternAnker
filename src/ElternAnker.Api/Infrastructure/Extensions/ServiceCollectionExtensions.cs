using ElternAnker.Api.Features.Ping;
using ElternAnker.Api.Infrastructure.Abstractions;

namespace ElternAnker.Api.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElternAnkerServices(this IServiceCollection services)
    {
        // Register feature handlers
        services.AddScoped<IQueryHandler<PingQuery, PingResponse>, PingHandler>();

        return services;
    }
}
