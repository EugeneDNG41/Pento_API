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
using Pento.Domain.MealPlanItems;

namespace Pento.Application.MealPlanItems.Get;
internal sealed class GetMealPlanItemQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        : IQueryHandler<GetMealPlanItemQuery, MealPlanItemResponse>
{
    public async Task<Result<MealPlanItemResponse>> Handle(
        GetMealPlanItemQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
                SELECT
                    id AS {nameof(MealPlanItemResponse.Id)},
                    meal_plan_id AS {nameof(MealPlanItemResponse.MealPlanId)},
                    recipe_id AS {nameof(MealPlanItemResponse.RecipeId)},
                    date AS {nameof(MealPlanItemResponse.Date)},
                    meal_type AS {nameof(MealPlanItemResponse.MealType)},
                    servings AS {nameof(MealPlanItemResponse.Servings)},
                    created_on_utc AS {nameof(MealPlanItemResponse.CreatedOnUtc)},
                    updated_on_utc AS {nameof(MealPlanItemResponse.UpdatedOnUtc)}
                FROM meal_plan_items
                WHERE id = @MealPlanItemId
                """;

        MealPlanItemResponse? mealPlanItem = await connection.QuerySingleOrDefaultAsync<MealPlanItemResponse>(
            sql,
            new { mealPlanItem = request.MealPlanItemId }
        );

        if (mealPlanItem is null)
        {
            return Result.Failure<MealPlanItemResponse>(MealPlanItemErrors.NotFound(request.MealPlanItemId));
        }

        return mealPlanItem;
    }
}

