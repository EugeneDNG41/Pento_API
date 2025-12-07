using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Reserve.Fullfill;

public sealed record FulfillMealPlanRecipeCommand(
    Guid MealPlanId,
    Guid RecipeId
) : ICommand<Guid>;
