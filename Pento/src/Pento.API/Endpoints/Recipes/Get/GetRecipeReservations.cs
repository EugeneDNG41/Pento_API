using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Recipes.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Recipes.Get;

internal sealed class GetRecipeReservations : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipe-reservations/current-house", async (
            IQueryHandler<GetRecipeReservationsByHouseholdIdQuery, IReadOnlyList<RecipeReservationResponse>> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetRecipeReservationsByHouseholdIdQuery();

            Result<IReadOnlyList<RecipeReservationResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Recipes)
        .RequireAuthorization();
    }
}
