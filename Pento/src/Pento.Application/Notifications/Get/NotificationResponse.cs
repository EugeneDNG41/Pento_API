using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.Notifications.Get;
public sealed record NotificationResponse(
    Guid Id,
    string Title,
    string Body,
    string Type,
    string? DataJson,
    DateTime SentOn,
    DateTime? ReadOn
);
