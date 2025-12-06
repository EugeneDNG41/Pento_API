using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.Households;

namespace Pento.Application.MealPlans.Reserve;

internal sealed class GetMealPlanReservationsByHouseholdHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext
)
    : IQueryHandler<GetMealPlanReservationsByHouseholdQuery, PagedList<MealPlanReservationResponse>>
{
    public async Task<Result<PagedList<MealPlanReservationResponse>>> Handle(
        GetMealPlanReservationsByHouseholdQuery query,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;

        if (householdId is null)
        {
            return Result.Failure<PagedList<MealPlanReservationResponse>>(HouseholdErrors.NotInAnyHouseHold);
        }

        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        var filters = new List<string>
        {
            "fir.household_id = @HouseholdId",
            "fir.reservation_for = 'MealPlan'"
        };

        var parameters = new DynamicParameters();
        parameters.Add("HouseholdId", householdId);

        if (query.MealType is not null)
        {
            filters.Add("mp.meal_type = @MealType");
            parameters.Add("MealType", query.MealType);
        }

        if (query.Date is not null)
        {
            filters.Add("mp.scheduled_date = @Date");
            parameters.Add("Date", query.Date.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (query.Month is not null)
        {
            filters.Add("EXTRACT(MONTH FROM mp.scheduled_date) = @Month");
            parameters.Add("Month", query.Month);
        }

        if (query.Year is not null)
        {
            filters.Add("EXTRACT(YEAR FROM mp.scheduled_date) = @Year");
            parameters.Add("Year", query.Year);
        }

        if (query.Status is not null)
        {
            filters.Add("fir.status = @Status");
            parameters.Add("Status", query.Status.ToString());
        }

        if (query.FoodReferenceId is not null)
        {
            filters.Add("fr.id = @FoodReferenceId");
            parameters.Add("FoodReferenceId", query.FoodReferenceId);
        }

        string where = "WHERE " + string.Join(" AND ", filters);

        string countSql = $"""
            SELECT COUNT(*)
            FROM food_item_reservations fir
            JOIN meal_plans mp ON mp.id = fir.meal_plan_id
            JOIN food_items fi ON fi.id = fir.food_item_id
            JOIN food_references fr ON fr.id = fi.food_reference_id
            {where};
        """;

        string dataSql = $"""
            SELECT
                fir.id                     AS Id,
                mp.id                      AS MealPlanId,
                mp.name                    AS MealPlanName,
                mp.meal_type               AS MealType,
                mp.scheduled_date          AS ScheduledDate,
                mp.servings                AS Servings,

                fi.id                      AS FoodItemId,
                fr.id                      AS FoodReferenceId,
                fr.name                    AS FoodReferenceName,
                fr.image_url               AS FoodReferenceImageUrl,
                fr.food_group              AS FoodGroup,

                fir.quantity               AS Quantity,
                u.abbreviation             AS UnitAbbreviation,

                fir.reservation_date_utc   AS ReservationDateUtc,
                fir.status                 AS Status

            FROM food_item_reservations fir
            JOIN meal_plans mp ON mp.id = fir.meal_plan_id
            JOIN food_items fi ON fi.id = fir.food_item_id
            JOIN food_references fr ON fr.id = fi.food_reference_id
            JOIN units u ON u.id = fir.unit_id
            {where}
            ORDER BY fir.reservation_date_utc DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        """;

        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        int totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var rows = (await connection.QueryAsync<MealPlanReservationResponse>(
            dataSql,
            parameters
        )).ToList();

        return Result.Success(PagedList<MealPlanReservationResponse>.Create(
            rows,
            totalCount,
            query.PageNumber,
            query.PageSize
        ));
    }
}
