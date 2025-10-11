using MediatR;
using Pento.API.Extensions;
using Pento.Application.MealPlanItems.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.MealPlanItems;

internal sealed class GetMealPlanItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meal-plan-items/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetMealPlanItemQuery(id);
            Result<MealPlanItemResponse> result = await sender.Send(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.MealPlanItems);
    }
}
