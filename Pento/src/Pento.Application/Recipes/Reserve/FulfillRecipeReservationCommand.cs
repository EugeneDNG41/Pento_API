using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Reserve;

public sealed record FulfillRecipeReservationCommand(
    Guid ReservationId,
    decimal NewQuantity,
    Guid UnitId
) : ICommand<Guid>;
