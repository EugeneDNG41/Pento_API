using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Notifications.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;

namespace Pento.API.Endpoints.Notifications.Get;

internal sealed class GetNotifications : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("notifications", async (
                  NotificationType? type,
                  SortOrder? sortOrder,
                  IQueryHandler<GetNotificationsQuery, PagedList<NotificationResponse>> handler,
                  CancellationToken cancellationToken,
                  int pageNumber = 1,
                  int pageSize = 10
              ) =>
        {
            var query = new GetNotificationsQuery(
                type,
                sortOrder,
                pageNumber,
                pageSize);

            Result<PagedList<NotificationResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem);
        })
              .RequireAuthorization()
              .WithTags(Tags.Notifications);
    }
}
