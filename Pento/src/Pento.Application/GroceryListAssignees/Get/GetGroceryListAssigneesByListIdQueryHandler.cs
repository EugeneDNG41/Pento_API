using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryListAssignees;

namespace Pento.Application.GroceryListAssignees.Get;

internal sealed class GetGroceryListAssigneesByListIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetGroceryListAssigneesByListIdQuery, IReadOnlyList<GroceryListAssigneeResponse>>
{
    public async Task<Result<IReadOnlyList<GroceryListAssigneeResponse>>> Handle(
        GetGroceryListAssigneesByListIdQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
            SELECT
                id AS {nameof(GroceryListAssigneeResponse.Id)},
                grocery_list_id AS {nameof(GroceryListAssigneeResponse.GroceryListId)},
                household_member_id AS {nameof(GroceryListAssigneeResponse.HouseholdMemberId)},
                is_completed AS {nameof(GroceryListAssigneeResponse.IsCompleted)},
                assigned_on_utc AS {nameof(GroceryListAssigneeResponse.AssignedOnUtc)}
            FROM grocery_list_assignee
            WHERE grocery_list_id = @GroceryListId
            ORDER BY assigned_on_utc DESC
            """;

        var assignees = (await connection.QueryAsync<GroceryListAssigneeResponse>(
            sql, new { request.GroceryListId }
        )).ToList();

        if (assignees.Count == 0)
        {
            return Result.Failure<IReadOnlyList<GroceryListAssigneeResponse>>(
                GroceryListAssigneeErrors.NotFoundByList(request.GroceryListId)
            );
        }

        return assignees;
    }
}
