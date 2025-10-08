using MediatR;
using Pento.API.Extensions;
using Pento.Application.MealPlans.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.MealPlans;

internal sealed class GetMealPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meal-plans/{mealPlanId:guid}", async (Guid mealPlanId, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetMealPlanQuery(mealPlanId);

            Result<MealPlanResponse> result = await sender.Send(query, cancellationToken);

            return result.Match(
                mealPlan => Results.Ok(mealPlan),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.MealPlans);
    }
}
