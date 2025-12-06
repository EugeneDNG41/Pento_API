using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.Recipes;

namespace Pento.Application.Recipes.Get;
internal sealed class GetRecipeReservationByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext
) : IQueryHandler<GetRecipeReservationByIdQuery, RecipeReservationDetailResponse>
{
    public async Task<Result<RecipeReservationDetailResponse>> Handle(
        GetRecipeReservationByIdQuery request,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<RecipeReservationDetailResponse>(
                HouseholdErrors.NotInAnyHouseHold);
        }

        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sqlReservation = """
            SELECT
                id AS Id,
                food_item_id AS FoodItemId,
                household_id AS HouseholdId,
                reservation_date_utc AS ReservationDateUtc,
                quantity AS Quantity,
                unit_id AS UnitId,
                status AS Status,
                recipe_id AS RecipeId
            FROM food_item_reservations
            WHERE id = @Id AND reservation_for = @For
        """;

        ReservationInfo? reservation = await connection.QueryFirstOrDefaultAsync<ReservationInfo>(
            sqlReservation,
            new { Id = request.ReservationId, For = ReservationFor.Recipe }
        );

        if (reservation is null || reservation.HouseholdId != householdId)
        {
            return Result.Failure<RecipeReservationDetailResponse>(
                RecipeErrors.NotFound);
        }

        const string sqlFoodItem = """
            SELECT
                id AS Id,
                name AS Name,
                quantity AS Quantity,
                unit_id AS UnitId,
                expiration_date AS ExpirationDate,
                image_url AS ImageUrl,
                notes AS Notes
            FROM food_items
            WHERE id = @FoodItemId
        """;

        FoodItemInfo? foodItem = await connection.QueryFirstOrDefaultAsync<FoodItemInfo>(
            sqlFoodItem,
            new { reservation.FoodItemId }
        );

        const string sqlRecipe = """
            SELECT
                id AS Id,
                title AS Title,
                description AS Description,
                prep_time_minutes AS PrepTimeMinutes,
                cook_time_minutes AS CookTimeMinutes,
                image_url AS ImageUrl,
                created_by AS CreatedBy
            FROM recipes
            WHERE id = @RecipeId
        """;

        RecipeInfo? recipe = await connection.QueryFirstOrDefaultAsync<RecipeInfo>(
            sqlRecipe,
            new { reservation.RecipeId }
        );

        if (foodItem is null)
        {
            return Result.Failure<RecipeReservationDetailResponse>(
                FoodItemErrors.NotFound
            );
        }

        if (recipe is null)
        {
            return Result.Failure<RecipeReservationDetailResponse>(
                RecipeErrors.NotFound
            );
        }

        var response = new RecipeReservationDetailResponse(reservation, foodItem, recipe);
        return Result.Success(response);
    }
}
