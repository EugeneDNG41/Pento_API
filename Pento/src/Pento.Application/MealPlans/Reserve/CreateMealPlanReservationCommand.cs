using Pento.Application.Abstractions.Messaging;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Reserve;


public sealed record CreateMealPlanReservationCommand(
    Guid FoodItemId,
    Guid? MealPlanId,
    decimal Quantity,
    Guid UnitId,
    MealType? MealType,
    DateOnly? ScheduledDate,
    int? Servings,     
    Guid? RecipeId     
) : ICommand<Guid>;

