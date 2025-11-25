using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
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
            $"""
            SELECT
                id AS {nameof(GroceryListItemResponse.Id)},
                list_id AS {nameof(GroceryListItemResponse.ListId)},
                food_ref_id AS {nameof(GroceryListItemResponse.FoodRefId)},
                custom_name AS {nameof(GroceryListItemResponse.CustomName)},
                quantity AS {nameof(GroceryListItemResponse.Quantity)},
                unit_id AS {nameof(GroceryListItemResponse.UnitId)},
                estimated_price AS {nameof(GroceryListItemResponse.EstimatedPrice)},
                notes AS {nameof(GroceryListItemResponse.Notes)},
                priority AS {nameof(GroceryListItemResponse.Priority)},
                added_by AS {nameof(GroceryListItemResponse.AddedBy)},
                created_on_utc AS {nameof(GroceryListItemResponse.CreatedOnUtc)}
            FROM grocery_list_items
            WHERE list_id = @ListId
            ORDER BY created_on_utc DESC
            """;

        var groceryListItems = (await connection.QueryAsync<GroceryListItemResponse>(
            sql, new { request.ListId }
        )).ToList();

        if (groceryListItems.Count == 0)
        {
            return Result.Failure<IReadOnlyList<GroceryListItemResponse>>(
                GroceryListItemErrors.NotFound
            );
        }

        return groceryListItems;
    }
}
