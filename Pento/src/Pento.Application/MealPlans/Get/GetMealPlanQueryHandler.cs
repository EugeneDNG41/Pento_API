using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Get;
internal sealed class GetMealPlanQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetMealPlanQuery, MealPlanResponse>
{
    public async Task<Result<MealPlanResponse>> Handle(GetMealPlanQuery request, CancellationToken cancellationToken)
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
            WHERE id = @MealPlanId
            """;

        MealPlanResponse? mealPlan = await connection.QuerySingleOrDefaultAsync<MealPlanResponse>(
            sql,
            new { MealPlanId = request.Id }
        );

        if (mealPlan is null)
        {
            return Result.Failure<MealPlanResponse>(MealPlanErrors.NotFound);
        }

        return mealPlan;
    }
}
