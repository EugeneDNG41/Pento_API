using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Notifications.Send;

public sealed record SendMealPlanReminderCommand(Guid MealPlanId)
    : ICommand;

