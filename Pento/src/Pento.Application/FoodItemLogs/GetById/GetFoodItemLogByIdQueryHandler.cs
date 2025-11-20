using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.GetById;
using Pento.Application.Users.Search;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemLogs;
using Pento.Domain.Households;

namespace Pento.Application.FoodItemLogs.GetById;

internal sealed class GetFoodItemLogByIdQueryHandler(IUserContext userContext, ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetFoodItemLogByIdQuery, FoodItemLogDetail>
{
    public async Task<Result<FoodItemLogDetail>> Handle(GetFoodItemLogByIdQuery query, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<FoodItemLogDetail>(HouseholdErrors.NotInAnyHouseHold);
        }
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();
        

        const string sql = """
            SELECT
                fil.id,
                fi.id AS FoodItemId,
                fi.name AS FoodItemName,
                fi.image_url AS FoodItemImageUrl,
                u.id AS UserId,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.avatar_url AS AvatarUrl,
                fil.timestamp,
                fil.action,
                fil.quantity,
                un.abbreviation AS UnitAbbreviation
            FROM food_item_logs fil
            INNER JOIN food_items fi ON fil.food_item_id = fi.Id
            INNER JOIN users u ON fil.user_id = u.Id
            INNER JOIN units un ON fil.unit_id = un.Id
            WHERE fil.Id = @Id AND fi.household_id = @HouseholdId;
            """;
        CommandDefinition command = new(sql, new { query.Id, HouseholdId = householdId }, cancellationToken: cancellationToken);
        FoodItemLogDetailRow? row = await connection.QuerySingleOrDefaultAsync<FoodItemLogDetailRow>(command);
        if (row is null)
        {
            return Result.Failure<FoodItemLogDetail>(FoodItemLogErrors.NotFound);
        }
        FoodItemLogDetail detail = new(
            row.Id,
            row.FoodItemId,
            row.FoodItemName,
            row.FoodItemImageUrl,
            new BasicUserResponse(
                row.UserId,
                row.FirstName,
                row.LastName,
                row.AvatarUrl),
            row.Timestamp,
            row.Action,
            row.Quantity,
            row.UnitAbbreviation);
        return detail;
    }
}
