using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Notifications;

namespace Pento.Application.Notifications.Get;

public sealed record GetNotificationsQuery(
    NotificationType? Type,
    SortOrder? SortOrder,
    int PageNumber,
    int PageSize
) : IQuery<PagedList<NotificationResponse>>;
