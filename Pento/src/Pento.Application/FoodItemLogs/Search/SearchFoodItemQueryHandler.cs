using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;

namespace Pento.Application.FoodItemLogs.Search;

internal sealed class SearchFoodItemQueryHandler(
    IUserContext userContext, 
    ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<SearchFoodItemLogQuery, PagedList<FoodItemLogPreview>>
{
    public async Task<Result<PagedList<FoodItemLogPreview>>> Handle(SearchFoodItemLogQuery query, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<PagedList<FoodItemLogPreview>>(HouseholdErrors.NotInAnyHouseHold);
        }
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync();
        var filters = new List<string>
        {
            "fil.is_deleted IS FALSE",
            "fil.is_archived IS FALSE",
            "fil.household_id = @HouseholdId"
        };
        var parameters = new DynamicParameters();
        parameters.Add("HouseholdId", householdId);
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            filters.Add("fi.name LIKE @Name");
            parameters.Add("Name", $"%{query.Name}%");
        }
        if (query.FromUtc is not null)
        {
            filters.Add("fil.timestamp >= @FromUtc");
            parameters.Add("FromUtc", query.FromUtc);
        }
        if (query.ToUtc is not null)
        {
            filters.Add("fil.timestamp <= @ToUtc");
            parameters.Add("ToUtc", query.ToUtc);
        }
        if (query.LogAction is not null)
        {
            filters.Add("fil.action = @LogAction");
            parameters.Add("LogAction", query.LogAction.ToString());
        }
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        string sql = @$"
                SELECT COUNT(*) 
                FROM food_item_logs fil
                INNER JOIN food_items fi ON fil.food_item_id = fi.Id
                {whereClause};
                SELECT
                    fil.id,
                    fi.name AS FoodItemName,
                    fi.image_url AS FoodItemImageUrl,
                    fil.timestamp,
                    fil.action,
                    fil.quantity,
                    u.abbreviation AS UnitAbbreviation
                FROM food_item_logs fil
                INNER JOIN food_items fi ON fil.food_item_id = fi.Id
                INNER JOIN units u ON fil.unit_id = u.Id
                {whereClause}
                ORDER BY fil.Timestamp DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;;
            ";
        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);
        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);
        int totalCount = await multi.ReadFirstAsync<int>();
        var foodItemLogs = (await multi.ReadAsync<FoodItemLogPreview>()).ToList();

        var pagedList = PagedList<FoodItemLogPreview>.Create(foodItemLogs, totalCount, query.PageNumber, query.PageSize);
        return Result.Success(pagedList);
    }
}
