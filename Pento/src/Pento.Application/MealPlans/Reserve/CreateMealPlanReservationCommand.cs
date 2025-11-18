using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Reserve;


public sealed record CreateMealPlanReservationCommand(
    Guid FoodItemId,
    Guid MealPlanId,
    decimal Quantity,
    Guid UnitId
) : ICommand<Guid>;
