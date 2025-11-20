using System.Data.Common;
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

        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

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

        string whereClause = filters.Count > 0
            ? "WHERE " + string.Join(" AND ", filters)
            : string.Empty;

        string orderBy = query.SortAsc
            ? "ORDER BY scheduled_date ASC"
            : "ORDER BY scheduled_date DESC";

        string sql = $@"
            SELECT COUNT(*)
            FROM meal_plans
            {whereClause};

            SELECT
                id AS Id,
                household_id AS HouseholdId,
                recipe_id AS RecipeId,
                food_item_id AS FoodItemId,
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
        ";

        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);

        int totalCount = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<MealPlanResponse>()).ToList();

        var paged = PagedList<MealPlanResponse>.Create(
            items,
            totalCount,
            query.PageNumber,
            query.PageSize
        );

        return Result.Success(paged);
    }
}

