using Pento.Application.Abstractions.Messaging;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Reserve;


public sealed record CreateMealPlanReservationCommand(
    Guid FoodItemId,
    decimal Quantity,
    Guid UnitId,
    MealType MealType,
    DateOnly ScheduledDate,
    int? Servings
) : ICommand<Guid>;

