using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Get;

internal sealed class GetMealPlansByHouseholdIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetMealPlansByHouseholdIdQuery, IReadOnlyList<MealPlanResponse>>
{
    public async Task<Result<IReadOnlyList<MealPlanResponse>>> Handle(
        GetMealPlansByHouseholdIdQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
            SELECT
                id AS {nameof(MealPlanResponse.Id)},
                household_id AS {nameof(MealPlanResponse.HouseholdId)},
                name AS {nameof(MealPlanResponse.Name)},
                created_by AS {nameof(MealPlanResponse.CreatedBy)},
                start_date AS {nameof(MealPlanResponse.StartDate)},
                end_date AS {nameof(MealPlanResponse.EndDate)},
                created_on_utc AS {nameof(MealPlanResponse.CreatedOnUtc)},
                updated_on_utc AS {nameof(MealPlanResponse.UpdatedOnUtc)}
            FROM meal_plans
            WHERE household_id = @HouseholdId
            ORDER BY start_date DESC
            """;

        var mealPlans = (await connection.QueryAsync<MealPlanResponse>(
            sql,
            new { request.HouseholdId }
        )).ToList();

        if (mealPlans.Count == 0)
        {
            return Result.Failure<IReadOnlyList<MealPlanResponse>>(
                MealPlanErrors.NotFoundByHousehold(request.HouseholdId)
            );
        }

        return mealPlans;
    }
}
