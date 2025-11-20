using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.MealPlans.Get;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;

namespace Pento.API.Endpoints.MealPlans.Get;

internal sealed class GetMealPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("meal-plans/{mealPlanId:guid}", async (Guid mealPlanId, IQueryHandler<GetMealPlanQuery, MealPlanDetailResponse> handler, CancellationToken cancellationToken) =>
        {
            var query = new GetMealPlanQuery(mealPlanId);

            Result<MealPlanDetailResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                mealPlan => Results.Ok(mealPlan),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.MealPlans);
        app.MapGet("meal-plans/current-house", async (
    int pageNumber,
    int pageSize,
    DateOnly? date,
    int? month,
    int? year,
    MealType? mealType,
    bool sortAsc,
    IQueryHandler<GetMealPlansByHouseholdIdQuery, PagedList<MealPlanResponse>> handler,
    CancellationToken cancellationToken
) =>
        {
            var query = new GetMealPlansByHouseholdIdQuery(
                PageNumber: pageNumber,
                PageSize: pageSize,
                Date: date,
                Month: month,
                Year: year,
                MealType: mealType,
                SortAsc: sortAsc
            );

            Result<PagedList<MealPlanResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
.WithTags("MealPlans")
.WithDescription("""
Query parameters:
- **date**: format `yyyy-MM-dd`
- **month**: 1–12
- **year**: e.g., 2024, 2025
- **mealType**: Breakfast / Lunch / Dinner / Snack
- **sortAsc**: true = earliest first
""")
.RequireAuthorization();
    }
}
