using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlanItems.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.MealPlanItems;

internal sealed class GetMealPlanItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meal-plan-items/{id:guid}", async (Guid id, IQueryHandler<GetMealPlanItemQuery, MealPlanItemResponse> handler, CancellationToken cancellationToken) =>
        {
            var query = new GetMealPlanItemQuery(id);
            Result<MealPlanItemResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.MealPlanItems);
    }
}
