using MediatR;
using Pento.Domain.Users;
using Pento.API.Extensions;
using Pento.Application.RecipeIngredients.Get;
using Pento.Domain.Abstractions;
using Pento.Infrastructure.Authentication;
namespace Pento.API.Endpoints.RecipeIngredients;

internal sealed class GetRecipeIngredientbyId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipe-ingredients/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetRecipeIngredientQuery(id);

            Result<RecipeIngredientResponse> result = await sender.Send(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
        .WithTags(Tags.RecipeIngredients);
    }
}
