using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryListItems;

namespace Pento.Application.GroceryListItems.Get;

internal sealed class GetGroceryListItemByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetGroceryListItemByIdQuery, GroceryListItemResponse>
{
    public async Task<Result<GroceryListItemResponse>> Handle(
        GetGroceryListItemByIdQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
            SELECT
                id AS {nameof(GroceryListItemResponse.Id)},
                list_id AS {nameof(GroceryListItemResponse.ListId)},
                food_ref_id AS {nameof(GroceryListItemResponse.FoodRefId)},
                custom_name AS {nameof(GroceryListItemResponse.CustomName)},
                quantity AS {nameof(GroceryListItemResponse.Quantity)},
                unit_id AS {nameof(GroceryListItemResponse.UnitId)},
                notes AS {nameof(GroceryListItemResponse.Notes)},
                priority AS {nameof(GroceryListItemResponse.Priority)},
                added_by AS {nameof(GroceryListItemResponse.AddedBy)},
                created_on_utc AS {nameof(GroceryListItemResponse.CreatedOnUtc)}
            FROM grocery_list_items
            WHERE id = @Id
            LIMIT 1
            """;

        GroceryListItemResponse? groceryListItem = await connection.QuerySingleOrDefaultAsync<GroceryListItemResponse>(
            sql, new { request.Id });

        if (groceryListItem is null)
        {
            return Result.Failure<GroceryListItemResponse>(
                GroceryListItemErrors.NotFound
            );
        }

        return groceryListItem;
    }
}
