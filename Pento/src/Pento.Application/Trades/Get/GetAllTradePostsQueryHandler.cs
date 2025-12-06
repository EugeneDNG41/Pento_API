using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.Abstractions;

namespace Pento.Application.Trades.Get;
internal sealed class GetAllTradePostsQueryHandler(ISqlConnectionFactory factory)
    : IQueryHandler<GetAllTradePostsQuery, PagedList<TradePostResponse>>
{
    public async Task<Result<PagedList<TradePostResponse>>> Handle(GetAllTradePostsQuery req, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await factory.OpenConnectionAsync(cancellationToken);

        var filters = new List<string>();
        var param = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            filters.Add("fr.name ILIKE @Search");
            param.Add("Search", $"%{req.Search}%");
        }

        filters.Add("o.is_deleted = false");

        string whereClause = filters.Count > 0
            ? "WHERE " + string.Join(" AND ", filters)
            : string.Empty;

        string orderBy = req.Sort?.ToLower(System.Globalization.CultureInfo.CurrentCulture) switch
        {
            "oldest" => "ORDER BY o.created_on ASC",
            "food" => "ORDER BY fr.name ASC",
            "food_desc" => "ORDER BY fr.name DESC",
            "quantity" => "ORDER BY i.quantity DESC",
            _ => "ORDER BY o.created_on DESC"
        };

        string sql = $@"
        -- Count
        SELECT COUNT(*) 
        FROM trade_items i
        JOIN trade_offers o ON i.offer_id = o.id
        JOIN food_items fi ON i.food_item_id = fi.id
        JOIN food_references fr ON fi.food_reference_id = fr.id
        JOIN units u ON i.unit_id = u.id
        {whereClause};

        -- Data
        SELECT 
            o.id AS OfferId,
            i.id AS ItemId,
            fi.id AS FoodItemId,
            fr.name AS FoodName,
            fr.image_url AS FoodImageUri,
            i.quantity AS Quantity,
            u.abbreviation AS UnitAbbreviation,
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
        {whereClause}
        {orderBy}
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ";

        param.Add("Offset", (req.PageNumber - 1) * req.PageSize);
        param.Add("PageSize", req.PageSize);

        SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, param);

        int totalCount = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<TradePostResponse>()).ToList();

        var paged = PagedList<TradePostResponse>.Create(
            items,
            totalCount,
            req.PageNumber,
            req.PageSize
        );

        return Result.Success(paged);
    }
}

