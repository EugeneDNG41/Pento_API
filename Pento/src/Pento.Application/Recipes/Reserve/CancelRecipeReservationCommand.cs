using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Reserve;

public sealed record CancelRecipeReservationCommand(Guid ReservationId)
    : ICommand<Guid>;
