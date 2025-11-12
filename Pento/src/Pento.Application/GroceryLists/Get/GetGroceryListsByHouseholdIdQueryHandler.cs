using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryLists;

namespace Pento.Application.GroceryLists.Get;

internal sealed class GetGroceryListsByHouseholdIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetGroceryListsByHouseholdIdQuery, IReadOnlyList<GroceryListResponse>>
{
    public async Task<Result<IReadOnlyList<GroceryListResponse>>> Handle(
        GetGroceryListsByHouseholdIdQuery request,
        CancellationToken cancellationToken)
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
            WHERE household_id = @HouseholdId
            ORDER BY updated_on_utc DESC
            """;

        var groceryLists = (await connection.QueryAsync<GroceryListResponse>(
            sql, new { request.HouseholdId }
        )).ToList();

        if (groceryLists.Count == 0)
        {
            return Result.Failure<IReadOnlyList<GroceryListResponse>>(
                GroceryListErrors.NotFoundByHousehold
            ); 
        }

        return groceryLists;
    }
}
