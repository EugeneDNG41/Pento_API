using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.ThirdPartyServices.Firebase;
using Pento.Domain.Abstractions;
using Pento.Domain.DeviceTokens;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;
using Pento.Domain.Notifications;

namespace Pento.Application.Notifications.Send;
internal sealed class SendMealPlanReminderCommandHandler(
    IGenericRepository<MealPlan> mealPlanRepo,
    IGenericRepository<DeviceToken> deviceTokenRepo,
    IGenericRepository<Notification> notificationRepo,
    INotificationSender fcm,
    IUserContext userContext,
    IUnitOfWork unitOfWork)
    : ICommandHandler<SendMealPlanReminderCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        SendMealPlanReminderCommand command,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        MealPlan? mealPlan = await mealPlanRepo.GetByIdAsync(command.MealPlanId, cancellationToken);
        if (mealPlan is null)
        {
            return Result.Failure<Guid>(MealPlanErrors.NotFound);
        }

        Guid userId = userContext.UserId;
        DeviceToken? token = (await deviceTokenRepo.FindAsync(
            x => x.UserId == userId, cancellationToken)).FirstOrDefault();

        if (token is null || string.IsNullOrWhiteSpace(token.Token))
        {
            return Result.Failure<Guid>(DeviceTokenErrors.NotFound);
        }

        var notification = Notification.Create(
            userId,
            "Meal plan reminder",
            $"{mealPlan.Name} is starting soon.",
            NotificationType.MealPlanReminder,
            dataJson: Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                mealPlanId = mealPlan.Id,
                mealPlanName = mealPlan.Name,
                startAt = mealPlan.ScheduledDate
            })
        );

        notificationRepo.Add(notification);

        await fcm.SendMealPlanReminderAsync(
            token.Token,
            mealPlan.Id,
            mealPlan.Name,
            mealPlan.ScheduledDate,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(notification.Id);
    }
}
