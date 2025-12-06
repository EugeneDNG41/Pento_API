using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeDirections.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.RecipeDirections.Get;

internal sealed class GetRecipeDirection : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipe-directions/{id:guid}", async (Guid id, IQueryHandler<GetRecipeDirectionQuery, RecipeDirectionResponse> handler, CancellationToken cancellationToken) =>
        {
            var query = new GetRecipeDirectionQuery(id);
            Result<RecipeDirectionResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.RecipeDirections);
    }
}
