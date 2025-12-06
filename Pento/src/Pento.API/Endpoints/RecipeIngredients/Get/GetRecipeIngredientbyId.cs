using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeIngredients.Get;
using Pento.Domain.Abstractions;
namespace Pento.API.Endpoints.RecipeIngredients.Get;

internal sealed class GetRecipeIngredientbyId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipe-ingredients/{id:guid}", async (Guid id, IQueryHandler<GetRecipeIngredientQuery, RecipeIngredientResponse> handler, CancellationToken cancellationToken) =>
        {
            var query = new GetRecipeIngredientQuery(id);

            Result<RecipeIngredientResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.RecipeIngredients);
    }
}
