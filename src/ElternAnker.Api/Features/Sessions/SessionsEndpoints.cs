using ElternAnker.Api.Features.Sessions.Chat;
using ElternAnker.Api.Features.Sessions.Feedback;
using ElternAnker.Api.Features.Sessions.StartSession;

namespace ElternAnker.Api.Features.Sessions;

public static class SessionsEndpoints
{
    public static IEndpointRouteBuilder MapSessionsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sessions")
            .WithTags("Sessions");

        group.MapStartSessionEndpoint();
        group.MapChatEndpoint();
        group.MapFeedbackEndpoint();

        return app;
    }
}

