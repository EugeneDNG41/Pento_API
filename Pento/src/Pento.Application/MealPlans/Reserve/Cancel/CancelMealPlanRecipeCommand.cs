using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Reserve.Cancel;

public sealed record CancelMealPlanRecipeCommand(
    Guid MealPlanId,
    Guid RecipeId
) : ICommand<Guid>;

