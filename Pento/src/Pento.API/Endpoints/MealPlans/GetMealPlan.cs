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
        app.MapGet("meal-plans/household/{householdId:guid}",
        async (Guid householdId, ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new GetMealPlansByHouseholdIdQuery(householdId);

                Result<IReadOnlyList<MealPlanResponse>> result = await sender.Send(query, cancellationToken);

                return result.Match(
                    mealPlans => Results.Ok(mealPlans),
                    CustomResults.Problem
                );
            })
            .WithTags(Tags.MealPlans);

    }

}
