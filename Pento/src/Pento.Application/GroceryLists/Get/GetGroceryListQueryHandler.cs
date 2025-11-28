using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.GroceryLists;

namespace Pento.Application.GroceryLists.Get;

internal sealed class GetGroceryListQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory)
    : IQueryHandler<GetGroceryListQuery, GroceryListDetailResponse>
{
    public async Task<Result<GroceryListDetailResponse>> Handle(
        GetGroceryListQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        const string sqlList = """
            SELECT
                id AS Id,
                household_id AS HouseholdId,
                name AS Name,
                created_by AS CreatedBy,
                created_on_utc AS CreatedOnUtc,
                updated_on_utc AS UpdatedOnUtc
            FROM grocery_list
            WHERE id = @Id
        """;

        GroceryListResponse? list = await connection.QueryFirstOrDefaultAsync<GroceryListResponse>(sqlList, new { request.Id });

        if (list is null)
        {
            return Result.Failure<GroceryListDetailResponse>(GroceryListErrors.NotFound);
        }
        const string sqlItems = """
            SELECT
                gli.id              AS Id,
                gli.food_ref_id     AS FoodRefId,
                fr.name             AS FoodRefName,
                fr.image_url        AS ImageUrl,
                gli.quantity        AS Quantity,
                gli.custom_name     AS CustomName,
                gli.unit_id         AS UnitId,
                u.abbreviation      AS UnitAbbreviation,
                gli.notes           AS Notes,
                gli.priority        AS Priority,
                gli.added_by        AS AddedBy,
                gli.created_on_utc  AS CreatedOnUtc
            FROM grocery_list_items gli
            JOIN food_references fr ON gli.food_ref_id = fr.id
            LEFT JOIN units u ON gli.unit_id = u.id
            WHERE gli.list_id = @Id
            ORDER BY gli.created_on_utc DESC;
        """;

        var items = (await connection.QueryAsync<GroceryListItemResponse>(
            sqlItems,
            new { request.Id })
        ).ToList();


        const string sqlAssignees = """
            SELECT
                id AS Id,
                household_member_id AS HouseholdMemberId,
                is_completed AS IsCompleted,
                assigned_on_utc AS AssignedOnUtc
            FROM grocery_list_assignees
            WHERE grocery_list_id = @Id
            ORDER BY assigned_on_utc DESC
        """;

        var assignees = (await connection.QueryAsync<GroceryListAssigneeResponse>(sqlAssignees, new { request.Id })).ToList();

        var detail = new GroceryListDetailResponse(
            Id: list.Id,
            HouseholdId: list.HouseholdId,
            Name: list.Name,
            CreatedBy: list.CreatedBy,
            CreatedOnUtc: list.CreatedOnUtc,
            UpdatedOnUtc: list.UpdatedOnUtc,
            Items: items,
            Assignees: assignees
        );

        return Result.Success(detail);
    }
}
