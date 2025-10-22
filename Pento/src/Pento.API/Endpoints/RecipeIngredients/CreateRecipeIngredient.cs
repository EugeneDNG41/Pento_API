using MediatR;
using Pento.API.Extensions;
using Pento.Application.RecipeIngredients.Create;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Endpoints.RecipeIngredients;

internal sealed class CreateRecipeIngredient : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("recipe-ingredients", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateRecipeIngredientCommand(
                request.RecipeId,
                request.FoodRefId,
                request.Quantity,
                request.UnitId,
                request.Notes
            );

            Result<Guid> result = await sender.Send(command, cancellationToken);

            return result.Match(
                id => Results.Ok(new { Id = id }),
                CustomResults.Problem
            );
        })
        .RequireAuthorization()
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
