using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Notifications;

namespace Pento.Application.Notifications.Create;

public sealed record CreateNotificationCommand(
    string Title,
    string Body,
    NotificationType Type,
    Dictionary<string, string>? Payload
) : ICommand;

