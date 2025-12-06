using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryListItems;

namespace Pento.Application.GroceryListItems.Get;

internal sealed class GetGroceryListItemsByListIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetGroceryListItemsByListIdQuery, IReadOnlyList<GroceryListItemResponse>>
{
    public async Task<Result<IReadOnlyList<GroceryListItemResponse>>> Handle(
        GetGroceryListItemsByListIdQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            """
            SELECT
                gli.id AS Id,
                gli.list_id AS ListId,
                gli.food_ref_id AS FoodRefId,
                gli.custom_name AS CustomName,
                gli.quantity AS Quantity,
                gli.unit_id AS UnitId,
                gli.notes AS Notes,
                gli.priority AS Priority,
                gli.added_by AS AddedBy,
                gli.created_on_utc AS CreatedOnUtc,

                fr.food_group AS FoodGroup,
                fr.typical_shelf_life_days_pantry AS TypicalShelfLifeDays_Pantry,
                fr.typical_shelf_life_days_fridge AS TypicalShelfLifeDays_Fridge,
                fr.typical_shelf_life_days_freezer AS TypicalShelfLifeDays_Freezer,
                fr.name AS FoodName,
                fr.image_url AS FoodImageUri

            FROM grocery_list_items gli
            LEFT JOIN food_references fr ON fr.id = gli.food_ref_id

            WHERE gli.list_id = @ListId
            ORDER BY gli.created_on_utc DESC
            """;

        var items = (await connection.QueryAsync<GroceryListItemResponse>(
            sql, new { request.ListId }
        )).ToList();


        return items;
    }
}

