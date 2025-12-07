using FirebaseAdmin.Messaging;
using Newtonsoft.Json;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.DeviceTokens;
using Pento.Domain.Households;
using Pento.Domain.Notifications;
using Pento.Domain.Users;
using Notification = Pento.Domain.Notifications.Notification;

namespace Pento.Infrastructure.External.Firebase;

public sealed class FcmNotificationService : INotificationService
{
    private readonly FirebaseMessaging firebaseMessaging;
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly IGenericRepository<DeviceToken> deviceTokenRepository;
    private readonly IGenericRepository<Notification> notificationRepository;
    private readonly IGenericRepository<User> userRepository;
    private readonly IGenericRepository<Household> householdRepository;
    private readonly IUnitOfWork unitOfWork;
    public FcmNotificationService(
        IDateTimeProvider dateTimeProvider,
        IGenericRepository<DeviceToken> deviceTokenRepository,
        IGenericRepository<Notification> notificationRepository,
        IGenericRepository<User> userRepository,
        IGenericRepository<Household> householdRepository,
        IUnitOfWork unitOfWork)
    {
        firebaseMessaging = FirebaseMessaging.DefaultInstance;
        this.dateTimeProvider = dateTimeProvider;
        this.deviceTokenRepository = deviceTokenRepository;
        this.notificationRepository = notificationRepository;
        this.userRepository = userRepository;
        this.householdRepository = householdRepository;
        this.unitOfWork = unitOfWork;
    }
    private async Task<Result> SendAsync(string deviceToken, string title, string body, Dictionary<string, string> data, CancellationToken cancellationToken)
    {
        try
        {
            var message = new Message
            {
                Token = deviceToken,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };
            await firebaseMessaging.SendAsync(message, cancellationToken);
        }
        catch (FirebaseMessagingException)
        {
            return Result.Failure(NotificationErrors.SendingFailed);
        }
        return Result.Success();
    }
    public async Task<Result> SendToUserAsync(
        Guid userId,
        string title,
        string body,
        NotificationType notificationType,
        Dictionary<string, string>? payload,
        CancellationToken cancellationToken)
    {
        payload ??= [];
        payload.Add("type", notificationType.ToString());
        var deviceTokens = (await deviceTokenRepository.FindAsync(dt => dt.UserId == userId, cancellationToken)).Select(dt => dt.Token).ToList();
        foreach (string deviceToken in deviceTokens)
        {
            Result result = await SendAsync(deviceToken, title, body, payload, cancellationToken);
            if (result.IsFailure)
            {
                return result;
            }
            var notification = Notification.Create(
                userId,
                title,
                body,
                notificationType,
                dateTimeProvider.UtcNow,
                JsonConvert.SerializeObject(payload));
            notificationRepository.Add(notification);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result> SendToHouseholdAsync(
        Guid householdId,
        string title,
        string body,
        NotificationType notificationType,
        Dictionary<string, string>? payload,
        CancellationToken cancellationToken)
    {
        Household? household = await householdRepository.GetByIdAsync(householdId, cancellationToken);
        if (household != null)
        {
            IEnumerable<User> users = await userRepository.FindAsync(u => u.HouseholdId == householdId, cancellationToken);
            foreach (User user in users)
            {
                Result result = await SendToUserAsync(user.Id, title, body, notificationType, payload, cancellationToken);
                if (result.IsFailure)
                {
                    return result;
                }
            }
        }
        return Result.Success();
    }
    public async Task<Result> SendUserMilestoneAchievedAsync(
        Guid userId,
        Guid milestoneId,
        string milestoneName,
        CancellationToken cancellationToken)
    {
        string title = "Milestone Achieved!";
        string body = $"Congratulations! You have achieved a new milestone: {milestoneName}.";
        var payload = new Dictionary<string, string>()
        {
            {  "milestoneId", milestoneId.ToString()  },
            { "milestoneName", milestoneName }
        };
        return await SendToUserAsync(userId, title, body, NotificationType.Milestone, payload, cancellationToken);
    }
}
