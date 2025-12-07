using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Get;

public sealed record GetRecipeReservationByIdQuery(Guid ReservationId)
    : IQuery<RecipeReservationDetailResponse>;
