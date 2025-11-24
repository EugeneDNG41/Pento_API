using Pento.Application.Abstractions.Messaging;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Reserve;


public sealed record CreateMealPlanReservationCommand(
    Guid FoodItemId,
    Guid? MealPlanId,   
    decimal Quantity,
    Guid UnitId,

    string? MealPlanName,
    MealType? MealType,
    DateOnly? ScheduledDate,
    int? Servings,
    string? Notes,
    Guid? RecipeId
) : ICommand<Guid>;

