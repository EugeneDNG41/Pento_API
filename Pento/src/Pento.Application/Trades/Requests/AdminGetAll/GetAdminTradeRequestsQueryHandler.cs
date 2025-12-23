using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Offers.AdminGetAll;
using Pento.Application.Trades.Requests.GetAll;
using Pento.Application.Users.GetAll;
using Pento.Domain.Abstractions;

namespace Pento.Application.Trades.Requests.AdminGetAll;

internal sealed class GetAdminTradeRequestsQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory
    ) : IQueryHandler<GetAdminTradeRequestsQuery, PagedList<TradeRequestAdminResponse>>
{
    public async Task<Result<PagedList<TradeRequestAdminResponse>>> Handle(
        GetAdminTradeRequestsQuery query,
        CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string orderBy = query.SortBy switch
        {
            GetTradeRequestsSortBy.CreatedOn => "tr.created_on",
            GetTradeRequestsSortBy.TotalItems => "12",
            _ => "tr.id"
        };
        string orderClause = $"ORDER BY {orderBy} {(query.SortOrder != null ? query.SortOrder.ToString() : "DESC")}";
        var parameters = new DynamicParameters();
        var filters = new List<string>();
        if (query.OfferId.HasValue)
        {
            parameters.Add("@OfferId", query.OfferId.Value);
            filters.Add("tr.trade_offer_id = @OfferId");
        }
        if (query.Status.HasValue)
        {
            parameters.Add("@Status", query.Status.Value.ToString());
            filters.Add("tr.status = @Status");
        }
        if (query.IsDeleted.HasValue)
        {
            parameters.Add("@IsDeleted", query.IsDeleted.Value);
            filters.Add("tr.is_deleted = @IsDeleted");
        }

        parameters.Add("@Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("@PageSize", query.PageSize);
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        string sql = $@"
            SELECT 
                tr.id AS TradeRequestId,
                tr.trade_offer_id AS TradeOfferId,
                tr.user_id AS UserId,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.avatar_url AS AvatarUrl,
                h1.name AS OfferHouseholdName,
                h2.name AS RequestHouseholdName,
                tr.status AS Status,
                tr.created_on AS CreatedOn,
                tr.updated_on AS UpdatedOn,
                COALESCE(
                    (SELECT COUNT(*) FROM trade_items ti
                     WHERE ti.request_id = tr.id), 0) AS TotalItems,
                tr.is_deleted AS IsDeleted
            FROM trade_requests tr
            JOIN trade_offers tof ON tr.trade_offer_id = tof.id
            JOIN households h1 ON tof.household_id = h1.id
            JOIN households h2 ON tr.household_id = h2.id
            JOIN users u ON tr.user_id = u.id
            {whereClause}
            {orderClause}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            SELECT COUNT(*) FROM trade_requests tr
            JOIN trade_offers tof ON tr.trade_offer_id = tof.id
            JOIN households h1 ON tof.household_id = h1.id
            JOIN households h2 ON tr.household_id = h2.id
            JOIN users u ON tr.user_id = u.id
            {whereClause};
        ";
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        IEnumerable<TradeRequestAdminRow> rows = await multi.ReadAsync<TradeRequestAdminRow>();

        var items = rows.Select(row => new TradeRequestAdminResponse
        {
            TradeRequestId = row.TradeRequestId,
            TradeOfferId = row.TradeOfferId,
            RequestUser = new BasicUserResponse(row.UserId, row.FirstName, row.LastName, row.AvatarUrl),
            OfferHouseholdName = row.OfferHouseholdName,
            RequestHouseholdName = row.RequestHouseholdName,
            Status = row.Status,
            CreatedOn = row.CreatedOn,
            UpdatedOn = row.UpdatedOn,
            TotalItems = row.TotalItems,
            IsDeleted = row.IsDeleted
        }).ToList();
        int totalCount = await multi.ReadFirstAsync<int>();
        var pagedList = PagedList<TradeRequestAdminResponse>.Create(
            items,
            count: totalCount,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize);
        return pagedList;
    }
}
