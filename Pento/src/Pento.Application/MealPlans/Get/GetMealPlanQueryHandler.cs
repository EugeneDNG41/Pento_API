using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Get;

internal sealed class GetMealPlanQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory
) : IQueryHandler<GetMealPlanQuery, MealPlanDetailResponse>
{
    public async Task<Result<MealPlanDetailResponse>> Handle(
        GetMealPlanQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection =
            await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        // 1️⃣ Lấy MealPlan
        const string sqlMealPlan = """
            SELECT 
                id,
                household_id,
                name,
                scheduled_date,
                meal_type,
                servings,
                notes,
                created_by,
                created_on_utc,
                updated_on_utc
            FROM meal_plans
            WHERE id = @MealPlanId;
        """;

        dynamic? mp = (await connection.QueryAsync(
            sqlMealPlan,
            new { MealPlanId = request.Id }
        )).FirstOrDefault();

        if (mp is null)
        {
            return Result.Failure<MealPlanDetailResponse>(MealPlanErrors.NotFound);
        }

        // 2️⃣ Lấy các recipe gắn với MealPlan (bảng meal_plan_recipes)
        const string sqlRecipes = """
            SELECT
                r.id,
                r.title,
                r.description,
                r.image_url,
                r.servings,
                r.difficulty_level
            FROM meal_plan_recipes mpr
            JOIN recipes r ON mpr.recipe_id = r.id
            WHERE mpr.meal_plan_id = @MealPlanId;
        """;

        IEnumerable<dynamic> recipeRows = await connection.QueryAsync(
            sqlRecipes,
            new { MealPlanId = request.Id }
        );

        var recipes = recipeRows
            .Select(r => new MealPlanRecipeInfo(
                Id: r.id,
                Title: r.title,
                Description: r.description,
                ImageUrl: r.image_url is string img && !string.IsNullOrWhiteSpace(img) ? new Uri(img) : null,
                Servings: (int?)r.servings,
                DifficultyLevel: r.difficulty_level
            ))
            .ToList();

        const string sqlFoodItems = """
            SELECT
                fir.food_item_id,
                fi.name            AS food_item_name,
                fr.name            AS food_reference_name,
                fr.food_group      AS food_group,
                fr.image_url       AS food_image_url,
                fi.quantity,
                u.abbreviation     AS unit_abbreviation,
                fi.expiration_date
            FROM food_item_reservations fir
            JOIN food_items fi         ON fir.food_item_id = fi.id
            JOIN food_references fr    ON fi.food_reference_id = fr.id
            JOIN units u               ON fi.unit_id = u.id
            WHERE fir.meal_plan_id = @MealPlanId;
        """;

        IEnumerable<dynamic> fiRows = await connection.QueryAsync(
            sqlFoodItems,
            new { MealPlanId = request.Id }
        );

        var foodItems = fiRows
            .Select(r => new MealPlanFoodItemInfo(
                Id: r.food_item_id,
                Name: r.food_item_name,
                FoodReferenceName: r.food_reference_name,
                FoodGroup: r.food_group,
                ImageUrl: r.food_image_url is string img && !string.IsNullOrWhiteSpace(img) ? new Uri(img) : null,
                Quantity: (decimal)r.quantity, // số lượng tồn kho hiện tại
                UnitAbbreviation: r.unit_abbreviation,
                ExpirationDate: r.expiration_date is DateTime dt
                    ? DateOnly.FromDateTime(dt)
                    : default
            ))
            .ToList();

        var response = new MealPlanDetailResponse(
            Id: mp.id,
            HouseholdId: mp.household_id,
            Name: mp.name,
            ScheduledDate: mp.scheduled_date is DateTime sd
                ? DateOnly.FromDateTime(sd)
                : mp.scheduled_date,
            MealType: mp.meal_type,
            Servings: mp.servings,
            Notes: mp.notes,
            CreatedBy: mp.created_by,
            CreatedOnUtc: mp.created_on_utc,
            UpdatedOnUtc: mp.updated_on_utc,
            Recipes: recipes,
            FoodItems: foodItems
        );

        return Result.Success(response);
    }
}
