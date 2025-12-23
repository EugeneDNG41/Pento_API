using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Trades.Sessions.GetAll;

internal sealed class GetTradeSessionsQueryHandler(
    IUserContext userContext,
    IGenericRepository<User> userRepository,
    ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetTradeSessionsQuery, PagedList<TradeSessionBasicResponse>>
{
    public async Task<Result<PagedList<TradeSessionBasicResponse>>> Handle(GetTradeSessionsQuery request, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId == null)
        {
            return Result.Failure<PagedList<TradeSessionBasicResponse>>(HouseholdErrors.NotInAnyHouseHold);
        }
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        var parameters = new DynamicParameters();
        var filters = new List<string>();
        parameters.Add("@HouseholdId", householdId.Value);
        filters.Add("(ts.offer_household_id = @HouseholdId OR ts.request_household_id = @HouseholdId)");
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
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            parameters.Add("@Search", $"%{request.Search.Trim()}%");
            filters.Add("(h.name ILIKE @Search OR h2.name ILIKE @Search OR fi.name ILIKE @Search OR fr.name ILIKE @Search)");
        }
        if (request.Status.HasValue)
        {
            parameters.Add("@Status", request.Status.Value.ToString());
            filters.Add("ts.status = @Status");
        }
        string whereClause = filters.Count > 0 ? "WHERE " + string.Join(" AND ", filters) : string.Empty;
        string orderByClause = request.SortOrder == SortOrder.ASC ? "ORDER BY ts.started_on ASC" : "ORDER BY ts.started_on DESC";
        int offset = (request.PageNumber - 1) * request.PageSize;
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@Offset", offset);
        string sql = $"""
            SELECT
              COUNT(*)
              FROM trade_sessions ts
              LEFT JOIN trade_session_items tsi ON tsi.id = ts.id
            JOIN households h ON h.id = ts.offer_household_id
            JOIN households h2 ON h2.id = ts.request_household_id
            LEFT JOIN food_items fi ON fi.id = tsi.food_item_id
            LEFT JOIN food_references fr ON fr.id = fi.food_reference_id
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
            LEFT JOIN food_items fi ON fi.id = tsi.food_item_id
            LEFT JOIN food_references fr ON fr.id = fi.food_reference_id
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
                .FindAsync(u => u.AvatarUrl != null && u.Id != userContext.UserId && (u.HouseholdId == item.OfferHouseholdId || u.HouseholdId == item.RequestHouseholdId), cancellationToken))
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
