using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;

namespace Pento.Application.Notifications.Get;

internal sealed class GetNotificationsQueryHandler(
    IUserContext userContext,
    IGenericRepository<Notification> notificationRepo
) : IQueryHandler<GetNotificationsQuery, List<NotificationResponse>>
{
    public async Task<Result<List<NotificationResponse>>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        IEnumerable<Notification> notifications = await notificationRepo.FindAsync(
            x => x.UserId == userId,
            cancellationToken
        );

        var list = notifications
            .OrderByDescending(n => n.SentOn)
            .Select(n => new NotificationResponse(
                n.Id,
                n.Title,
                n.Body,
                n.Type.ToString(),
                n.DataJson,
                n.SentOn,
                n.ReadOn
            ))
            .ToList();

        return Result.Success(list);
    }
}
