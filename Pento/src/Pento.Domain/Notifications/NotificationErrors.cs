using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Notifications;
public static class NotificationErrors
{
    public static readonly Error NotFound = Error.NotFound(
       code: "Notification.NotificationNotFound",
       description: "The specified notification was not found."
   );
}
