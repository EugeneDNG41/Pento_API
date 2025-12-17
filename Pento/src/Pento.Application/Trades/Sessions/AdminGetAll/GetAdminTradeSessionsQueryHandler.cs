using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Sessions.GetAll;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;

namespace Pento.Application.Trades.Sessions.AdminGetAll;

internal sealed class GetAdminTradeSessionsQueryHandler(
    IGenericRepository<User> userRepository,
    ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetAdminTradeSessionsQuery, PagedList<TradeSessionBasicResponse>>
{
    public async Task<Result<PagedList<TradeSessionBasicResponse>>> Handle(GetAdminTradeSessionsQuery request, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        var parameters = new DynamicParameters();
        var filters = new List<string>();
        if (request.OfferId.HasValue)
        {
            parameters.Add("@OfferId", request.OfferId.Value);
            filters.Add("ts.trade_offer_id = @OfferId");
        }
        if (request.RequestId.HasValue)
        {
            parameters.Add("@RequestId", request.RequestId.Value);
            filters.Add("ts.trade_request_id = @RequestId");
        }
        if (request.Status.HasValue)
        {
            parameters.Add("@Status", request.Status.Value.ToString());
            filters.Add("ts.status = @Status");
        }
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        string orderByClause = request.SortOrder == SortOrder.DESC ? "ORDER BY ts.started_on DESC" : "ORDER BY ts.started_on ASC";
        int offset = (request.PageNumber - 1) * request.PageSize;
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@Offset", offset);
        string sql = $"""
            SELECT
              COUNT(*)
              FROM trade_sessions ts
                {whereClause};
            SELECT
              ts.id                                   AS TradeSessionId,
              ts.trade_offer_id                       AS TradeOfferId,
              ts.trade_request_id                     AS TradeRequestId,
              ts.offer_household_id                   AS OfferHouseholdId,
              h.name                                 AS OfferHouseholdName,
              ts.request_household_id                 AS RequestHouseholdId,
              h2.name                                AS RequestHouseholdName,
              ts.status                               AS Status,
              ts.started_on                           AS StartedOn,
              COALESCE(
                (SELECT COUNT(*) FROM trade_session_items tsi
                 WHERE tsi.session_id = ts.id
                   AND tsi.from = 'Offer'), 0)         AS TotalOfferedItems,
            
              COALESCE(
                (SELECT COUNT(*) FROM trade_session_items tsi
                 WHERE tsi.session_id = ts.id
                   AND tsi.from = 'Request'), 0)       AS TotalRequestedItems,
              ts.confirmed_by_offer_user_id 			AS ConfirmedByOfferUserId,
              ts.confirmed_by_request_user_id			AS ConfirmedByRequestUserId
            FROM trade_sessions ts
            LEFT JOIN trade_session_items tsi ON tsi.id = ts.id
            JOIN households h ON h.id = ts.offer_household_id
            JOIN households h2 ON h2.id = ts.request_household_id
            {whereClause}
            {orderByClause}
            LIMIT @PageSize OFFSET @Offset;           
            """;
        CommandDefinition command = new(sql, parameters, cancellationToken: cancellationToken);
        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(command);
        int totalCount = await multi.ReadFirstAsync<int>();

        IEnumerable<TradeSessionRow> items = await multi.ReadAsync<TradeSessionRow>();
        var responses = new List<TradeSessionBasicResponse>();
        foreach (TradeSessionRow item in items)
        {
            var userAvatars = (await userRepository
                .FindAsync(u => u.AvatarUrl != null && (u.HouseholdId == item.OfferHouseholdId || u.HouseholdId == item.RequestHouseholdId), cancellationToken))
                .Select(u => u.AvatarUrl!)
                .ToList();
            List<Uri> avatarUrls = userAvatars.Count > 0 ? userAvatars! : new();
            var response = new TradeSessionBasicResponse(
                item.TradeSessionId,
                item.Status,
                item.StartedOn,
                item.TotalOfferedItems,
                item.TotalRequestedItems,
                avatarUrls);
            responses.Add(response);
        }
        var pagedList = new PagedList<TradeSessionBasicResponse>(
            responses,
            totalCount,
            request.PageNumber,
            request.PageSize);
        return pagedList;

    }
}
