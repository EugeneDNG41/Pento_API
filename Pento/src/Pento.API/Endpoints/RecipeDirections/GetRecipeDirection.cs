using MediatR;
using Pento.API.Extensions;
using Pento.Application.RecipeDirections.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.RecipeDirections;

internal sealed class GetRecipeDirection : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("recipe-directions/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetRecipeDirectionQuery(id);
            Result<RecipeDirectionResponse> result = await sender.Send(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.RecipeDirections);
    }
}
