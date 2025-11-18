using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.Households;

namespace Pento.Application.MealPlans.Reserve;

internal sealed class GetMealPlanReservationsByHouseholdIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext
) : IQueryHandler<GetMealPlanReservationsByHouseholdIdQuery, IReadOnlyList<MealPlanReservationResponse>>
{
    public async Task<Result<IReadOnlyList<MealPlanReservationResponse>>> Handle(
        GetMealPlanReservationsByHouseholdIdQuery request,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;

        if (householdId is null)
        {
            return Result.Failure<IReadOnlyList<MealPlanReservationResponse>>(
                HouseholdErrors.NotInAnyHouseHold
            );
        }

        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        const string sql = """
            SELECT
                id AS Id,
                food_item_id AS FoodItemId,
                meal_plan_id AS MealPlanId,
                quantity AS Quantity,
                unit_id AS UnitId,
                reservation_date_utc AS ReservationDateUtc,
                status AS Status
            FROM food_item_reservations
            WHERE household_id = @HouseholdId
              AND reservation_for = @MealPlanEnum
            ORDER BY reservation_date_utc DESC
        """;

        var reservations = (await connection.QueryAsync<MealPlanReservationResponse>(
            sql,
            new
            {
                HouseholdId = householdId,
                MealPlanEnum = ReservationFor.MealPlan
            }
        )).ToList();

        return reservations;
    }
}
