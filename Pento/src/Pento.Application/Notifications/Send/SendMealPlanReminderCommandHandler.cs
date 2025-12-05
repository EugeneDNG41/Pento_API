using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.ThirdPartyServices.Firebase;
using Pento.Domain.Abstractions;
using Pento.Domain.DeviceTokens;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;
using Pento.Domain.Notifications;
using Pento.Domain.Subscriptions;
using Pento.Domain.UserSubscriptions;

namespace Pento.Application.Notifications.Send;
internal sealed class SendMealPlanReminderCommandHandler(
    IGenericRepository<MealPlan> mealPlanRepo,
    INotificationService fcm,
    IUserContext userContext)
    : ICommandHandler<SendMealPlanReminderCommand>
{
    public async Task<Result> Handle(
        SendMealPlanReminderCommand command,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure(HouseholdErrors.NotInAnyHouseHold);
        }

        MealPlan? mealPlan = await mealPlanRepo.GetByIdAsync(command.MealPlanId, cancellationToken);
        if (mealPlan is null)
        {
            return Result.Failure(MealPlanErrors.NotFound);
        }


        var payload = new Dictionary<string, string>
        {
            { "mealPlanId", mealPlan.Id.ToString() },
            { "mealPlanName", mealPlan.Name }
        };

        await fcm.SendToUserAsync(
            userContext.UserId,
            "Meal plan reminder",
            $"{mealPlan.Name} is starting soon.",
            NotificationType.MealPlanReminder,
            payload,
            cancellationToken);


        return Result.Success();
    }
}
