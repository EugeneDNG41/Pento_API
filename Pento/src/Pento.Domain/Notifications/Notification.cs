using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Notifications;
public sealed class Notification : Entity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public string Body { get; private set; }
    public NotificationType Type { get; private set; }
    public string? DataJson { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime? SentOnUtc { get; private set; }
    public DateTime? ReadOnUtc { get; private set; }

    private Notification() { }

    private Notification(
        Guid id,
        Guid userId,
        string title,
        string body,
        NotificationType type,
        string? dataJson)
        : base(id)
    {
        UserId = userId;
        Title = title;
        Body = body;
        Type = type;
        DataJson = dataJson;
        Status = NotificationStatus.Pending;
        SentOnUtc = DateTime.UtcNow;

    }

    public static Notification Create(
        Guid userId,
        string title,
        string body,
        NotificationType type,
        string? dataJson = null)
    {
        return new Notification(
            Guid.NewGuid(),
            userId,
            title,
            body,
            type,
            dataJson
        );
    }

    public void MarkSent()
    {
        Status = NotificationStatus.Sent;
        SentOnUtc = DateTime.UtcNow;
    }


}
