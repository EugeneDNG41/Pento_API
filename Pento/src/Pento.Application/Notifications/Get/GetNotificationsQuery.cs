using Pento.Application.Abstractions.Messaging;
using Pento.Application.Notifications.Get;
namespace Pento.Application.Notifications.Get;
public sealed record GetNotificationsQuery() : IQuery<List<NotificationResponse>>;
