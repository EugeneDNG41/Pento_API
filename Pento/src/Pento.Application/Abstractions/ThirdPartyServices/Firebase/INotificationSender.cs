using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.Abstractions.ThirdPartyServices.Firebase;
public interface INotificationSender
{
    Task SendAsync(string deviceToken, string title, string body, CancellationToken ct);
}

