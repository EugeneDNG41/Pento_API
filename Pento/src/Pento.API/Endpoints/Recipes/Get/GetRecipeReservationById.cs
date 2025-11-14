using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItemReservations.Get;
using Pento.Application.Recipes.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Get;

internal sealed class GetRecipeReservationById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipe-reservations/{id:guid}", async (
            Guid id,
            IQueryHandler<GetRecipeReservationByIdQuery, RecipeReservationDetailResponse> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetRecipeReservationByIdQuery(id);

            Result<RecipeReservationDetailResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Recipes)
        .RequireAuthorization();
    }
}
