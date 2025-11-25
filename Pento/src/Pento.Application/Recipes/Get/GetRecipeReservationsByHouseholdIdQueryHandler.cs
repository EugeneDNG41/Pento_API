using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.Households;
using Pento.Domain.Recipes;

namespace Pento.Application.FoodItemReservations.Get;

internal sealed class GetRecipeReservationsByHouseholdIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext
) : IQueryHandler<GetRecipeReservationsByHouseholdIdQuery, IReadOnlyList<RecipeReservationResponse>>
{
    public async Task<Result<IReadOnlyList<RecipeReservationResponse>>> Handle(
        GetRecipeReservationsByHouseholdIdQuery request,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;

        if (householdId is null)
        {
            return (Result<IReadOnlyList<RecipeReservationResponse>>)Result.Failure(HouseholdErrors.NotInAnyHouseHold);
        }

        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql = """
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
            WHERE household_id = @HouseholdId
              AND reservation_for = @ReservationFor
            ORDER BY reservation_date_utc DESC
        """;
        CommandDefinition command = new(
            sql,
            new { HouseholdId = householdId.Value, ReservationFor = ReservationFor.Recipe },
            cancellationToken: cancellationToken
        );
        var reservations = (await connection.QueryAsync<RecipeReservationResponse>(command)).ToList();


        if (reservations.Count == 0)
        {
            return Result.Failure<IReadOnlyList<RecipeReservationResponse>>(RecipeErrors.NotFound);
        }

        return reservations;
    }
}
