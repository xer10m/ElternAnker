using ElternAnker.Infrastructure.Abstractions;
using ElternAnker.Infrastructure.Rag;
using ElternAnker.Infrastructure.StateMachine;
using ElternAnker.Infrastructure.Storage;
using ElternAnker.Infrastructure.VectorStore;
using Microsoft.Extensions.DependencyInjection;

namespace ElternAnker.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IPedagogyVectorStore, InMemoryVectorStore>();
        services.AddSingleton<ISessionStore, InMemorySessionStore>();
        services.AddScoped<IRagEngine, StubRagEngine>();
        services.AddScoped<IDialogStateMachine, DialogStateMachine>();

        return services;
    }
}

