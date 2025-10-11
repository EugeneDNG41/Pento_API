using MediatR;
using Pento.API.Extensions;
using Pento.Application.MealPlanItems.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.MealPlanItems;

internal sealed class CreateMealPlanItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("meal-plan-items", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateMealPlanItemCommand(
                request.MealPlanId,
                request.RecipeId,
                request.MealType,
                request.Schedule,
                request.Servings,
                request.Notes
            );

            Result<Guid> result = await sender.Send(command, cancellationToken);

            return result.Match(
                id => Results.Created($"/meal-plan-items/{id}", id),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.MealPlanItems);
    }

    internal sealed class Request
    {
        public Guid MealPlanId { get; init; }

        public Guid RecipeId { get; init; }


        public string MealType { get; init; } = string.Empty;


        public List<DateTime> Schedule { get; init; } = new();

        public int Servings { get; init; }

        public string? Notes { get; init; }
    }
}
