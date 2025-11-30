using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Get;

public sealed record GetRecipeReservationsByHouseholdIdQuery()
    : IQuery<IReadOnlyList<RecipeReservationResponse>>;
