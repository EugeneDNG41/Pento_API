using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.ThirdPartyServices.Firebase;
using Pento.Application.Abstractions.UtilityServices.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.DeviceTokens;
using Pento.Domain.FoodItems;
using Pento.Domain.Notifications;
using Quartz;

namespace Pento.Infrastructure.ThirdPartyServices.Quartz;

[DisallowConcurrentExecution]
internal sealed class ProcessExpirationDateTrackingJob(
    IGenericRepository<FoodItem> foodItemRepository,
    IGenericRepository<DeviceToken> deviceTokenRepository,
    IGenericRepository<Notification> notificationRepository,
    INotificationSender notificationSender,
    IUnitOfWork unitOfWork
) : IJob
{
    private const int EXPIRE_SOON_HOURS = 72;

    public async Task Execute(IJobExecutionContext context)
    {
        DateTime now = DateTime.UtcNow;

        var foodItems = (await foodItemRepository.FindAsync(
            fi => fi.Quantity > 0,
            context.CancellationToken)).ToList();

        foreach (FoodItem? foodItem in foodItems)
        {
            var exp = foodItem.ExpirationDate.ToDateTime(TimeOnly.MinValue);
            TimeSpan diff = exp - now;

            if (diff <= TimeSpan.Zero || diff > TimeSpan.FromHours(EXPIRE_SOON_HOURS))
            {
                continue;
            }

            Notification? recentNoti = (await notificationRepository.FindAsync(
                n =>
                    n.Type == NotificationType.FoodExpiringSoon &&
                    n.UserId == foodItem.AddedBy &&
                    n.DataJson != null &&
                    n.DataJson.Contains(foodItem.Id.ToString()) &&
                    n.SentOnUtc != null &&
                    n.SentOnUtc > now.AddHours(-24),
                context.CancellationToken))
                .FirstOrDefault();

            if (recentNoti != null)
            {
                continue;
            }

            string? deviceToken = (await deviceTokenRepository.FindAsync(
                t => t.UserId == foodItem.AddedBy,
                context.CancellationToken))
                .FirstOrDefault()?.Token;

            if (string.IsNullOrEmpty(deviceToken))
            {
                continue;
            }

            await notificationSender.SendFoodExpiringSoonAsync(
                deviceToken,
                foodItem.Id,
                foodItem.Name,
                (int)diff.TotalHours,
                context.CancellationToken);

            var data = new
            {
                foodItemId = foodItem.Id,
                name = foodItem.Name,
                hoursLeft = (int)diff.TotalHours,
                expiration = foodItem.ExpirationDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)
            };

            var noti = Notification.Create(
                foodItem.AddedBy,
                $"{foodItem.Name} is expiring soon",
                $"This item will expire in {(int)diff.TotalHours} hours.",
                NotificationType.FoodExpiringSoon,
                Newtonsoft.Json.JsonConvert.SerializeObject(data)
            );

            noti.MarkSent();
            notificationRepository.Add(noti);
        }

        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
