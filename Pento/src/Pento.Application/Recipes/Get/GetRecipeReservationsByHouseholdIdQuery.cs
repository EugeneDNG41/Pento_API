using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItemReservations.Get;

public sealed record GetRecipeReservationsByHouseholdIdQuery()
    : IQuery<IReadOnlyList<RecipeReservationResponse>>;
