using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;

namespace Pento.Application.Trades.Get;

internal sealed class GetAllTradePostsQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory factory)
    : IQueryHandler<GetAllTradePostsQuery, PagedList<TradePostGroupedResponse>>
{
    public async Task<Result<PagedList<TradePostGroupedResponse>>> Handle(GetAllTradePostsQuery query, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<PagedList<TradePostGroupedResponse>>(HouseholdErrors.NotInAnyHouseHold);

        }
        await using DbConnection connection = await factory.OpenConnectionAsync(cancellationToken);

        var filters = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            filters.Add("fr.name ILIKE @Search");
            parameters.Add("Search", $"%{query.Search}%");
        }
        if (query.IsMyHousehold.HasValue)
        {
            if (query.IsMyHousehold.Value)
            {
                parameters.Add("@HouseholdId", householdId.Value);
                filters.Add("o.household_id = @HouseholdId");
            }
            else
            {
                parameters.Add("@HouseholdId", householdId.Value);
                filters.Add("o.household_id <> @HouseholdId");
            }
        }
        if (query.IsMine.HasValue)
        {
            if (query.IsMine.Value)
            {
                parameters.Add("@UserId", userContext.UserId);
                filters.Add("o.user_id = @UserId");
            }
            else
            {
                parameters.Add("@UserId", userContext.UserId);
                filters.Add("o.user_id <> @UserId");
            }

        }

        filters.Add("o.is_deleted = false");

        string whereClause = filters.Count > 0
            ? "WHERE " + string.Join(" AND ", filters)
            : string.Empty;

        string orderBy = query.Sort?.ToLower(System.Globalization.CultureInfo.CurrentCulture) switch
        {
            "oldest" => "ORDER BY o.created_on ASC",
            "food" => "ORDER BY fr.name ASC",
            "food_desc" => "ORDER BY fr.name DESC",
            "quantity" => "ORDER BY i.quantity DESC",
            _ => "ORDER BY o.created_on DESC"
        };

        string sql = $@"
        -- Count offers (distinct)
        SELECT COUNT(DISTINCT o.id)
        FROM trade_items i
        JOIN trade_offers o ON i.offer_id = o.id
        JOIN food_items fi ON i.food_item_id = fi.id
        JOIN food_references fr ON fi.food_reference_id = fr.id
        JOIN units u ON i.unit_id = u.id
        JOIN users us ON o.user_id = us.id
        {whereClause};

        -- Data
        SELECT 
            o.id AS OfferId,
            i.id AS ItemId,
            fi.id AS FoodItemId,
            fr.name AS FoodName,
            fr.image_url AS FoodImageUri,
            us.first_name AS PostedByName,
            us.avatar_url AS PostedByAvatarUrl,
            i.quantity AS Quantity,
            u.abbreviation AS UnitAbbreviation,
            o.status AS Status,
            o.start_date AS StartDate,
            o.end_date AS EndDate,
            o.pickup_option AS PickupOption,
            o.user_id AS PostedBy,
            o.created_on AS CreatedOnUtc
        FROM trade_items i
        JOIN trade_offers o ON i.offer_id = o.id
        JOIN food_items fi ON i.food_item_id = fi.id
        JOIN food_references fr ON fi.food_reference_id = fr.id
        JOIN units u ON i.unit_id = u.id
        JOIN users us ON o.user_id = us.id
        {whereClause}
        {orderBy}
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ";

        parameters.Add("Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("PageSize", query.PageSize);

        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);

        int totalCount = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<TradePostResponse>()).ToList();

        var grouped = items
            .GroupBy(x => x.OfferId)
            .Select(g => new TradePostGroupedResponse(
                OfferId: g.Key,
                Status: g.First().Status,
                StartDate: g.First().StartDate,
                EndDate: g.First().EndDate,
                PickupOption: g.First().PickupOption,
                PostedBy: g.First().PostedBy,
                CreatedOnUtc: g.First().CreatedOnUtc,
                PostedByName: g.First().PostedByName,
                PostedByAvatarUrl: g.First().PostedByAvatarUrl,
                Items: g.Select(x => new TradePostItemResponse(
                    ItemId: x.ItemId,
                    FoodItemId: x.FoodItemId,
                    FoodName: x.FoodName,
                    FoodImageUri: x.FoodImageUri,
                    Quantity: x.Quantity,
                    UnitAbbreviation: x.UnitAbbreviation
                )).ToList()
            ))
            .ToList();

        var paged = PagedList<TradePostGroupedResponse>.Create(
            grouped,
            totalCount,
            query.PageNumber,
            query.PageSize
        );

        return Result.Success(paged);
    }
}
