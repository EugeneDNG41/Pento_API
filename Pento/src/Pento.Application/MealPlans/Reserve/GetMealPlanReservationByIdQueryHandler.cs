using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Reserve;

internal sealed class GetMealPlanReservationByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext
) : IQueryHandler<GetMealPlanReservationByIdQuery, MealPlanReservationDetailResponse>
{
    public async Task<Result<MealPlanReservationDetailResponse>> Handle(
        GetMealPlanReservationByIdQuery request,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<MealPlanReservationDetailResponse>(
                HouseholdErrors.NotInAnyHouseHold
            );
        }

        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sqlReservation = """
            SELECT
                id AS Id,
                food_item_id AS FoodItemId,
                meal_plan_id AS MealPlanId,
                quantity AS Quantity,
                unit_id AS UnitId,
                reservation_date_utc AS ReservationDateUtc,
                status AS Status
            FROM food_item_reservations
            WHERE id = @Id 
        """;

        MealPlanReservationInfo? reservation = await connection.QueryFirstOrDefaultAsync<MealPlanReservationInfo>(
            sqlReservation,
            new { request.Id, MealPlanEnum = ReservationFor.MealPlan.ToString() }
        );

        if (reservation is null)
        {
            return Result.Failure<MealPlanReservationDetailResponse>(
                FoodItemReservationErrors.NotFound
            );
        }

        const string sqlFoodItem = """
            SELECT
                id AS Id,
                name AS Name,
                quantity AS Quantity,
                unit_id AS UnitId,
                expiration_date AS ExpirationDate,
                image_url AS ImageUrl
            FROM food_items
            WHERE id = @FoodItemId
        """;

        FoodItemInfo? foodItem = await connection.QueryFirstOrDefaultAsync<FoodItemInfo>(
            sqlFoodItem,
            new { reservation.FoodItemId }
        );

        if (foodItem is null)
        {
            return Result.Failure<MealPlanReservationDetailResponse>(
                FoodItemErrors.NotFound
            );
        }

        const string sqlMealPlan = """
            SELECT
                id AS Id,
                name AS Name,
                meal_type AS MealType,
                scheduled_date AS ScheduledDate,
                servings AS Servings,
                notes AS Notes
            FROM meal_plans
            WHERE id = @MealPlanId
        """;

        MealPlanInfo? mealPlan = await connection.QueryFirstOrDefaultAsync<MealPlanInfo>(
            sqlMealPlan,
            new { reservation.MealPlanId }
        );

        if (mealPlan is null)
        {
            return Result.Failure<MealPlanReservationDetailResponse>(
                MealPlanErrors.NotFound
            );
        }

        var response = new MealPlanReservationDetailResponse(
            reservation,
            foodItem,
            mealPlan
        );

        return Result.Success(response);
    }
}
