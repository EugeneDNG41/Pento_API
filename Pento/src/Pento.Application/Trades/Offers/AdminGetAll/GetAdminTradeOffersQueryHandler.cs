using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Requests.AdminGetAll;
using Pento.Application.Users.GetAll;
using Pento.Domain.Abstractions;

namespace Pento.Application.Trades.Offers.AdminGetAll;

internal sealed class GetAdminTradeOffersQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory
    ) : IQueryHandler<GetAdminTradeOffersQuery, PagedList<TradeOfferAdminResponse>>
{
    public async Task<Result<PagedList<TradeOfferAdminResponse>>> Handle(
        GetAdminTradeOffersQuery query,
        CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string orderBy = query.SortBy switch
        {
            GetTradeOffersSortBy.CreatedOn => "tof.created_on",
            GetTradeOffersSortBy.TotalItems => "10",
            GetTradeOffersSortBy.TotalRequests => "11",
            _ => "tof.id"
        };
        string orderClause = $"ORDER BY {orderBy} {(query.SortOrder != null ? query.SortOrder.ToString() : "ASC")}";
        var parameters = new DynamicParameters();
        var filters = new List<string>();
        if (query.Status.HasValue)
        {
            parameters.Add("@Status", query.Status.Value.ToString());
            filters.Add("tof.status = @Status");
        }
        if (query.IsDeleted.HasValue)
        {
            parameters.Add("@IsDeleted", query.IsDeleted.Value);
            filters.Add("tof.is_deleted = @IsDeleted");
        }

        parameters.Add("@Offset", (query.PageNumber - 1) * query.PageSize);
        parameters.Add("@PageSize", query.PageSize);
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        string sql = $@"
            SELECT 
                tof.id AS TradeOfferId,
                tof.user_id AS UserId,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.avatar_url AS AvatarUrl,
                h1.name AS OfferHouseholdName,
                tof.status AS Status,
                tof.created_on AS CreatedOn,
                tof.updated_on AS UpdatedOn,
                COALESCE(
                    (SELECT COUNT(*) FROM trade_items ti
                     WHERE ti.offer_id = tof.id), 0) AS TotalItems,
                Coalesce(
                    (SELECT COUNT(*) FROM trade_requests tr
                     WHERE tr.trade_offer_id = tof.id), 0) AS TotalRequests,
                tof.is_deleted AS IsDeleted
            FROM trade_offers tof
            JOIN trade_requests tr ON tr.trade_offer_id = tof.id
            JOIN households h1 ON tof.household_id = h1.id
            JOIN users u ON tof.user_id = u.id
            {whereClause}
            {orderClause}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            SELECT COUNT(*) FROM trade_offers tof
            JOIN trade_requests tr ON tr.trade_offer_id = tof.id
            JOIN households h1 ON tof.household_id = h1.id
            JOIN users u ON tof.user_id = u.id
            {whereClause};
        ";
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        IEnumerable<TradeOfferAdminRow> rows = await multi.ReadAsync<TradeOfferAdminRow>();

        var items = rows.Select(row => new TradeOfferAdminResponse
        {
            TradeOfferId = row.TradeOfferId,
            OfferUser = new BasicUserResponse(row.UserId, row.FirstName, row.LastName, row.AvatarUrl),
            OfferHouseholdName = row.OfferHouseholdName,
            Status = row.Status,
            CreatedOn = row.CreatedOn,
            UpdatedOn = row.UpdatedOn,
            TotalItems = row.TotalItems,
            TotalRequests = row.TotalRequests,
            IsDeleted = row.IsDeleted
        }).ToList();
        int totalCount = await multi.ReadFirstAsync<int>();
        var pagedList = PagedList<TradeOfferAdminResponse>.Create(
            items,
            count: totalCount,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize);
        return pagedList;
    }
}
