using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Reserve.Cancel;

public sealed record CancelMealPlanCommand(
    Guid MealPlanId
) : ICommand<Guid>;
