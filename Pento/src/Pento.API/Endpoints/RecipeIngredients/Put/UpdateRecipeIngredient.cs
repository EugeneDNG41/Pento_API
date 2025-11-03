using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeIngredients.Update;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;
namespace Pento.API.Endpoints.RecipeIngredients.Put;

internal sealed class UpdateRecipeIngredient : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("recipe-ingredients/{recipeIngredientId:guid}", async (
            Guid recipeIngredientId,
            Request request,
            ICommandHandler<UpdateRecipeIngredientCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(
        new UpdateRecipeIngredientCommand(recipeIngredientId, request.Quantity, request.UnitId, request.Notes), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.RecipeIngredients);
    }
internal sealed class Request
{
    public decimal Quantity { get; init; }
    public string? Notes { get; init; }
    public Guid UnitId { get; init; }

    }

}
