using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Reserve.Fullfill;

public sealed record FulfillMealPlanCommand(
    Guid MealPlanId
) : ICommand<Guid>;
