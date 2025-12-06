using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Notifications;

namespace Pento.Application.Abstractions.External.Firebase;
public interface INotificationService
{
    Task<Result> SendToHouseholdAsync(Guid householdId, string title, string body, NotificationType notificationType, Dictionary<string, string>? payload, CancellationToken cancellationToken);
    Task<Result> SendToUserAsync(Guid userId, string title, string body, NotificationType notificationType, Dictionary<string, string>? payload, CancellationToken cancellationToken);
}

