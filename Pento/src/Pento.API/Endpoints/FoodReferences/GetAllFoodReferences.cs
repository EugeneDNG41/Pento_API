using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodReferences.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.API.Endpoints.FoodReferences;

internal sealed class GetAllFoodReferences : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("food-references", async (
            [AsParameters] QueryParams queryParams,
            IQueryHandler<GetAllFoodReferencesQuery, IReadOnlyList<FoodReferenceResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllFoodReferencesQuery(queryParams.FoodGroup);

            Result<IReadOnlyList<FoodReferenceResponse>> result =
                await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.FoodReferences);
    }

    internal sealed class QueryParams
    {
        public FoodGroup? FoodGroup { get; init; }
    }
}
