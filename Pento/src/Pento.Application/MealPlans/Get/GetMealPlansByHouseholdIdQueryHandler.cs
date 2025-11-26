using System.Data.Common;
using System.Linq;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Get;


internal sealed class GetMealPlansByHouseholdIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext
)
    : IQueryHandler<GetMealPlansByHouseholdIdQuery, PagedList<MealPlanResponse>>
{
    public async Task<Result<PagedList<MealPlanResponse>>> Handle(
        GetMealPlansByHouseholdIdQuery query,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<PagedList<MealPlanResponse>>(HouseholdErrors.NotInAnyHouseHold);
        }

        await using DbConnection connection =
            await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        var filters = new List<string> { "household_id = @HouseholdId" };
        var parameters = new DynamicParameters();
        parameters.Add("HouseholdId", householdId);

        if (query.Date is not null)
        {
            filters.Add("scheduled_date = @Date");
            parameters.Add("Date", query.Date.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (query.Month is not null)
        {
            filters.Add("EXTRACT(MONTH FROM scheduled_date) = @Month");
            parameters.Add("Month", query.Month);
        }

        if (query.Year is not null)
        {
            filters.Add("EXTRACT(YEAR FROM scheduled_date) = @Year");
            parameters.Add("Year", query.Year);
        }

        if (query.MealType is not null)
        {
            filters.Add("meal_type = @MealType");
            parameters.Add("MealType", query.MealType.Value.ToString());
        }

        string whereClause = "WHERE " + string.Join(" AND ", filters);

        string orderBy = query.SortAsc
            ? "ORDER BY scheduled_date ASC"
            : "ORDER BY scheduled_date DESC";

        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        string sql = $@"
            -- Count
            SELECT COUNT(*)
            FROM meal_plans
            {whereClause};

            -- MealPlans
            SELECT
                id AS Id,
                household_id AS HouseholdId,
                name AS Name,
                scheduled_date AS ScheduledDate,
                meal_type AS MealType,
                servings AS Servings,
                notes AS Notes,
                created_by AS CreatedBy,
                created_on_utc AS CreatedOnUtc,
                updated_on_utc AS UpdatedOnUtc
            FROM meal_plans
            {whereClause}
            {orderBy}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            -- Recipes
            SELECT
                mpr.meal_plan_id,
                r.id,
                r.title,
                r.description,
                r.image_url,
                r.servings,
                r.difficulty_level
            FROM meal_plan_recipes mpr
            JOIN recipes r ON mpr.recipe_id = r.id
            WHERE mpr.meal_plan_id IN (
                SELECT id FROM meal_plans {whereClause}
                {orderBy}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            );

            -- Recipe ingredients (food_ref_ids)
            SELECT
                mpr.meal_plan_id,
                ri.food_ref_id
            FROM meal_plan_recipes mpr
            JOIN recipe_ingredients ri ON mpr.recipe_id = ri.recipe_id
            WHERE mpr.meal_plan_id IN (
                SELECT id FROM meal_plans {whereClause}
                {orderBy}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            );

            -- FoodItem reservations
            SELECT
                fir.meal_plan_id          AS MealPlanId,
                fi.id                     AS FoodItemId,
                fr.id                     AS FoodReferenceId,
                fi.name                   AS FoodItemName,
                fr.name                   AS FoodReferenceName,
                fr.food_group             AS FoodGroup,
                fr.image_url              AS FoodImageUrl,
                fi.quantity               AS Quantity,
                u.abbreviation            AS UnitAbbreviation,
                fi.expiration_date        AS ExpirationDate
            FROM food_item_reservations fir
            JOIN food_items fi ON fir.food_item_id = fi.id
            JOIN food_references fr ON fi.food_reference_id = fr.id
            JOIN units u ON fi.unit_id = u.id
            WHERE fir.meal_plan_id IN (
                SELECT id FROM meal_plans {whereClause}
                {orderBy}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
            );
        ";

        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);

        int totalCount = await multi.ReadFirstAsync<int>();
        var mealPlans = (await multi.ReadAsync<MealPlanRow>()).ToList();

        var recipeRows = (await multi.ReadAsync()).ToList();
        var ingredientRows = (await multi.ReadAsync()).ToList();

        var foodItemRows = (await multi.ReadAsync<MealPlanFoodItemRow>()).ToList();
        var recipeLookup = recipeRows
            .GroupBy(r => (Guid)r.meal_plan_id)
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => new MealPlanRecipeInfo(
                        (Guid)r.id,
                        (string)r.title,
                        (string?)r.description,
                        r.image_url is string img && !string.IsNullOrWhiteSpace(img) ? new Uri(img) : null,
                        (int?)r.servings,
                        (string?)r.difficulty_level
                )).ToList()
            );

        var ingredientRefLookup = ingredientRows
            .GroupBy(r => (Guid)r.meal_plan_id)
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => (Guid)r.food_ref_id).ToHashSet()
            );

        var foodItemLookup = foodItemRows
            .GroupBy(f => f.MealPlanId)
            .ToDictionary(
                g => g.Key,
                g => g.ToList()
            );

        var list = mealPlans.Select(mp =>
        {
            recipeLookup.TryGetValue(mp.Id, out List<MealPlanRecipeInfo>? recipes);
            recipes ??= new List<MealPlanRecipeInfo>();

            ingredientRefLookup.TryGetValue(mp.Id, out HashSet<Guid>? ingredientFoodRefs);
            ingredientFoodRefs ??= new HashSet<Guid>();
            MealType mealtype = Enum.Parse<MealType>(mp.MealType);
            foodItemLookup.TryGetValue(mp.Id, out List<MealPlanFoodItemRow>? allFoodItems);
            allFoodItems ??= new List<MealPlanFoodItemRow>();

            var directFoodItems = allFoodItems
                .Where(fi => !ingredientFoodRefs.Contains(fi.FoodReferenceId))
                .Select(fi => new MealPlanFoodItemInfo(
                    fi.FoodItemId,
                    fi.FoodItemName,
                    fi.FoodReferenceName,
                    fi.FoodGroup,
                    fi.FoodImageUrl is string img && !string.IsNullOrWhiteSpace(img) ? new Uri(img) : null,
                    fi.Quantity,
                    fi.UnitAbbreviation,
                    fi.ExpirationDate is DateTime dt ? DateOnly.FromDateTime(dt) : default
                ))
                .ToList();

            return new MealPlanResponse(
                mp.Id,
                mp.HouseholdId,
                mp.Name,
                DateOnly.FromDateTime(mp.ScheduledDate),
                mealtype.ToString(),
                mp.Servings,
                mp.Notes,
                mp.CreatedBy,
                mp.CreatedOnUtc,
                mp.UpdatedOnUtc,
                recipes,
                directFoodItems
            );
        }).ToList();

        return Result.Success(
            PagedList<MealPlanResponse>.Create(list, totalCount, query.PageNumber, query.PageSize)
        );
    }
}






