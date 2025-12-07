using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Reserve.Cancel;

public sealed record CancelMealPlanReservationCommand(Guid ReservationId)
    : ICommand<Guid>;
