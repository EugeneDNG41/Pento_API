using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Reserve.Fullfill;

public sealed record FulfillMealPlanReservationCommand(
    Guid ReservationId,
    decimal NewQuantity,
    Guid UnitId
) : ICommand<Guid>;
