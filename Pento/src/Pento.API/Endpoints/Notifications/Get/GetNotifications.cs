using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Notifications.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Notifications.Get;

internal sealed class GetNotifications : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("notifications", async (
            IQueryHandler<GetNotificationsQuery, List<NotificationResponse>> handler,
            CancellationToken cancellationToken
        ) =>
        {
            Result<List<NotificationResponse>> result =
                await handler.Handle(new GetNotificationsQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Notifications)
        .RequireAuthorization();
    }
}
