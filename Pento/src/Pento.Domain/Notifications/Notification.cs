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
    public DateTime SentOn { get; private set; }
    public DateTime? ReadOn { get; private set; }

    private Notification() { }

    private Notification(
        Guid id,
        Guid userId,
        string title,
        string body,
        NotificationType type,
        string? dataJson,
        DateTime sentOn
        )
        : base(id)
    {
        UserId = userId;
        Title = title;
        Body = body;
        Type = type;
        DataJson = dataJson;
        SentOn = sentOn;

    }

    public static Notification Create(
        Guid userId,
        string title,
        string body,
        NotificationType type,
        DateTime sentOn,
        string? dataJson = null)
    {
        return new Notification(
            Guid.CreateVersion7(),
            userId,
            title,
            body,
            type,
            dataJson,
            sentOn
        );
    }
}
