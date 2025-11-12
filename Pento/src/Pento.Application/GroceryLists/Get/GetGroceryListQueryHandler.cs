using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryLists;

namespace Pento.Application.GroceryLists.Get;

internal sealed class GetGroceryListQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetGroceryListQuery, GroceryListResponse>
{
    public async Task<Result<GroceryListResponse>> Handle(GetGroceryListQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
            SELECT
                id AS {nameof(GroceryListResponse.Id)},
                household_id AS {nameof(GroceryListResponse.HouseholdId)},
                name AS {nameof(GroceryListResponse.Name)},
                created_by AS {nameof(GroceryListResponse.CreatedBy)},
                created_on_utc AS {nameof(GroceryListResponse.CreatedOnUtc)},
                updated_on_utc AS {nameof(GroceryListResponse.UpdatedOnUtc)}
            FROM grocery_list
            WHERE id = @Id
            """;

        GroceryListResponse? groceryList = await connection.QueryFirstOrDefaultAsync<GroceryListResponse>(
            sql, new { request.Id });

        if (groceryList is null)
        {
            return Result.Failure<GroceryListResponse>(GroceryListErrors.NotFound);
        }

        return Result.Success(groceryList);
    }
}
