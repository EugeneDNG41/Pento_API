using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Pento.Application.Abstractions.ThirdPartyServices.Firebase;

namespace Pento.Infrastructure.ThirdPartyServices.Firebase;
public sealed class FcmNotificationSender : INotificationSender
{
    private readonly FirebaseMessaging _firebaseMessaging;
    public FcmNotificationSender()
    {
        _firebaseMessaging = FirebaseMessaging.DefaultInstance;
    }
    public async Task SendAsync(string deviceToken, string title, string body, CancellationToken ct)
    {
        var message = new Message
        {
            Token = deviceToken,
            Notification = new Notification
            {
                Title = title,
                Body = body
            }
        };
        await _firebaseMessaging.SendAsync(message, ct);
    }
}
