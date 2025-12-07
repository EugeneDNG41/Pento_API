using Pento.Application.Abstractions.Messaging;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Create;

public sealed record CreateMealPlanCommand(
    Guid? RecipeId,
    Guid? FoodItemId,
    string Name,
    MealType MealType,
    DateOnly ScheduledDate,
    int Servings,
    string? Notes
) : ICommand<Guid>;
