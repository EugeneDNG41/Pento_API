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
                recipe_id AS {nameof(MealPlanResponse.RecipeId)},
                food_item_id AS {nameof(MealPlanResponse.FoodItemId)},
                name AS {nameof(MealPlanResponse.Name)},
                scheduled_date AS {nameof(MealPlanResponse.ScheduledDate)},
                meal_type AS {nameof(MealPlanResponse.MealType)},
                servings AS {nameof(MealPlanResponse.Servings)},
                notes AS {nameof(MealPlanResponse.Notes)},
                created_by AS {nameof(MealPlanResponse.CreatedBy)},
                created_on_utc AS {nameof(MealPlanResponse.CreatedOnUtc)},
                updated_on_utc AS {nameof(MealPlanResponse.UpdatedOnUtc)}
            FROM meal_plans
            WHERE household_id = @HouseholdId
            ORDER BY scheduled_date DESC
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
