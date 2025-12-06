using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.RecipeIngredients.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.RecipeIngredients.Post;

internal sealed class CreateRecipeIngredient : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("recipe-ingredients", async (Request request, ICommandHandler<CreateRecipeIngredientCommand, Guid> handler, CancellationToken cancellationToken) =>
        {
            var command = new CreateRecipeIngredientCommand(
                request.RecipeId,
                request.FoodRefId,
                request.Quantity,
                request.UnitId,
                request.Notes
            );

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.RecipeIngredients);
    }

    internal sealed class Request
    {
        public Guid RecipeId { get; init; }
        public Guid FoodRefId { get; init; }
        public decimal Quantity { get; init; }
        public Guid UnitId { get; init; }
        public string? Notes { get; init; }
    }
}
