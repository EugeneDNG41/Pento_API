using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryListAssignees;

namespace Pento.Application.GroceryListAssignees.Get;

internal sealed class GetGroceryListAssigneeByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetGroceryListAssigneeByIdQuery, GroceryListAssigneeResponse>
{
    public async Task<Result<GroceryListAssigneeResponse>> Handle(
        GetGroceryListAssigneeByIdQuery request,
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
            WHERE id = @Id
            LIMIT 1
            """;

        GroceryListAssigneeResponse? assignee = await connection.QuerySingleOrDefaultAsync<GroceryListAssigneeResponse>(
            sql, new { request.Id });

        if (assignee is null)
        {
            return Result.Failure<GroceryListAssigneeResponse>(
                GroceryListAssigneeErrors.NotFound
            );
        }

        return assignee;
    }
}
