using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryLists;

namespace Pento.Application.GroceryLists.Get;

internal sealed class GetGroceryListsByHouseholdIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory,
    IUserContext userContext
)
    : IQueryHandler<GetGroceryListsByHouseholdIdQuery, IReadOnlyList<GroceryListResponse>>
{
    public async Task<Result<IReadOnlyList<GroceryListResponse>>> Handle(
        GetGroceryListsByHouseholdIdQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        Guid? householdId = userContext.HouseholdId;

        if (householdId is null)
        {
            return Result.Failure<IReadOnlyList<GroceryListResponse>>(GroceryListErrors.ForbiddenAccess);
        }

        const string sql = """
            SELECT
                id AS Id,
                household_id AS HouseholdId,
                name AS Name,
                created_by AS CreatedBy,
                created_on_utc AS CreatedOnUtc,
                updated_on_utc AS UpdatedOnUtc
            FROM grocery_list
            WHERE household_id = @HouseholdId
            ORDER BY updated_on_utc DESC
        """;

        var groceryLists = (await connection.QueryAsync<GroceryListResponse>(
            sql, new { HouseholdId = householdId }
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
