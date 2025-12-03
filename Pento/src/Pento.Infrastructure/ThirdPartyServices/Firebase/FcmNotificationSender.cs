using FirebaseAdmin.Messaging;
using Newtonsoft.Json;
using Pento.Application.Abstractions.ThirdPartyServices.Firebase;
using Pento.Domain.Notifications;

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
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = body
            }
        };

        await _firebaseMessaging.SendAsync(message, ct);
    }


    public async Task SendByTypeAsync(
        string deviceToken,
        NotificationType type,
        string title,
        string body,
        string? dataJson,
        CancellationToken ct)
    {
        var message = new Message
        {
            Token = deviceToken,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = body
            },
            Data = BuildData(type, dataJson)
        };

        await _firebaseMessaging.SendAsync(message, ct);
    }


    private static Dictionary<string, string> BuildData(NotificationType type, string? dataJson)
    {
        var data = new Dictionary<string, string>
        {
            { "type", type.ToString() }
        };

        if (!string.IsNullOrWhiteSpace(dataJson))
        {
            try
            {
                Dictionary<string, string>? extras = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataJson);
                if (extras != null)
                {
                    foreach (KeyValuePair<string, string> kv in extras)
                    {
                        data[kv.Key] = kv.Value;
                    }
                }
            }
            catch
            {
                data["rawData"] = dataJson;
            }
        }

        return data;
    }


    public Task SendGeneralAsync(
        string deviceToken,
        string title,
        string body,
        CancellationToken ct)
    {
        return SendByTypeAsync(
            deviceToken,
            NotificationType.General,
            title,
            body,
            null,
            ct);
    }

    public Task SendFoodExpiringSoonAsync(
        string deviceToken,
        Guid foodItemId,
        string name,
        int hoursLeft,
        CancellationToken ct)
    {
        var data = new
        {
            foodItemId,
            name,
            hoursLeft
        };

        return SendByTypeAsync(
            deviceToken,
            NotificationType.FoodExpiringSoon,
            $"{name} is expiring soon",
            $"This item will expire in {hoursLeft} hours.",
            JsonConvert.SerializeObject(data),
            ct);
    }

    public Task SendFoodReservationAsync(
        string deviceToken,
        Guid reservationId,
        Guid foodItemId,
        string name,
        CancellationToken ct)
    {
        var data = new
        {
            reservationId,
            foodItemId,
            name
        };

        return SendByTypeAsync(
            deviceToken,
            NotificationType.FoodReservation,
            "Reservation confirmed",
            $"{name} reserved successfully.",
            JsonConvert.SerializeObject(data),
            ct);
    }

    public Task SendMealPlanReminderAsync(
        string deviceToken,
        Guid mealPlanId,
        string mealPlanName,
        DateOnly startAt,
        CancellationToken ct)
    {
        var data = new
        {
            mealPlanId,
            mealPlanName,
            startAt
        };

        return SendByTypeAsync(
            deviceToken,
            NotificationType.MealPlanReminder,
            $"Meal plan reminder",
            $"{mealPlanName} is starting soon.",
            JsonConvert.SerializeObject(data),
            ct);
    }

    public Task SendGiveawayPostAsync(
        string deviceToken,
        Guid giveawayId,
        string title,
        CancellationToken ct)
    {
        var data = new
        {
            giveawayId,
            title
        };

        return SendByTypeAsync(
            deviceToken,
            NotificationType.GiveawayPost,
            $"New giveaway posted",
            title,
            JsonConvert.SerializeObject(data),
            ct);
    }

    public Task SendGiveawayClaimAsync(
        string deviceToken,
        Guid claimId,
        Guid giveawayId,
        string claimerName,
        CancellationToken ct)
    {
        var data = new
        {
            claimId,
            giveawayId,
            claimerName
        };

        return SendByTypeAsync(
            deviceToken,
            NotificationType.GiveawayClaim,
            "Your giveaway is claimed",
            $"{claimerName} has claimed your giveaway.",
            JsonConvert.SerializeObject(data),
            ct);
    }

    public Task SendSubscriptionAsync(
        string deviceToken,
        string status,
        DateTime? expireAt,
        CancellationToken ct)
    {
        var data = new
        {
            status,
            expireAt
        };

        return SendByTypeAsync(
            deviceToken,
            NotificationType.Subscription,
            $"Subscription {status}",
            $"Your subscription status updated.",
            JsonConvert.SerializeObject(data),
            ct);
    }
}
