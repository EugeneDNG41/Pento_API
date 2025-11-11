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
    public async Task<Result<MealPlanDetailResponse>> Handle(GetMealPlanQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        const string sql = """
            SELECT 
                mp.id, 
                mp.household_id, 
                mp.name, 
                mp.scheduled_date, 
                mp.meal_type, 
                mp.servings, 
                mp.notes, 
                mp.created_by, 
                mp.created_on_utc, 
                mp.updated_on_utc,
                -- Recipe
                r.id AS RecipeId,
                r.title, 
                r.description, 
                r.image_url, 
                r.servings AS RecipeServings, 
                r.difficulty_level,
                -- FoodItem
                fi.id AS FoodItemId,
                fi.name AS FoodItemName,
                fr.name AS FoodReferenceName,
                fr.food_group AS FoodGroup,
                fr.image_url AS FoodImageUrl,
                fi.quantity, 
                u.abbreviation AS UnitAbbreviation,
                fi.expiration_date
            FROM meal_plans mp
            LEFT JOIN recipes r ON mp.recipe_id = r.id
            LEFT JOIN food_items fi ON mp.food_item_id = fi.id
            LEFT JOIN food_references fr ON fi.food_reference_id = fr.id
            LEFT JOIN units u ON fi.unit_id = u.id
            WHERE mp.id = @MealPlanId
        """;

        IEnumerable<dynamic> result = await connection.QueryAsync<dynamic>(sql, new { MealPlanId = request.Id });

        dynamic? record = result.FirstOrDefault();
        if (record is null)
        {
            return Result.Failure<MealPlanDetailResponse>(MealPlanErrors.NotFound);
        }
        RecipeInfo? recipe = null;
        if (record.recipeid is not null)
        {
            recipe = new RecipeInfo(
                Id: record.recipeid,
                Title: record.title,
                Description: record.description,
                ImageUrl: record.image_url is not null ? new Uri(record.image_url) : null,
                Servings: record.recipeservings,
                DifficultyLevel: record.difficulty_level
            );
        }

        FoodItemInfo? foodItem = null;
        if (record.fooditemid is not null)
        {
            foodItem = new FoodItemInfo(
                Id: record.fooditemid,
                Name: record.fooditemname,
                FoodReferenceName: record.foodreferencename,
                FoodGroup: record.foodgroup,
                ImageUrl: record.foodimageurl is not null ? new Uri(record.foodimageurl) : null,
                Quantity: record.quantity,
                UnitAbbreviation: record.unitabbreviation,
                ExpirationDate: record.expiration_date
            );
        }

        var mealPlan = new MealPlanDetailResponse(
            Id: record.id,
            HouseholdId: record.household_id,
            Name: record.name,
            ScheduledDate: record.scheduled_date,
            MealType: record.meal_type,
            Servings: record.servings,
            Notes: record.notes,
            CreatedBy: record.created_by,
            CreatedOnUtc: record.created_on_utc,
            UpdatedOnUtc: record.updated_on_utc,
            Recipe: recipe,
            FoodItem: foodItem
        );

        return mealPlan;
    }
}
