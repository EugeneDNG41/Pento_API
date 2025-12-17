using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Application.Users.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;
using Pento.Domain.Users;

namespace Pento.Application.Trades.Sessions.AdminGetById;

internal sealed class GetAdminTradeSessionByIdQueryHandler( 
    IGenericRepository<User> userRepository,
    ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetAdminTradeSessionByIdQuery, TradeSessionDetailResponse>
{
    public async Task<Result<TradeSessionDetailResponse>> Handle(GetAdminTradeSessionByIdQuery request, CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        var parameters = new DynamicParameters();
        parameters.Add("@TradeSessionId", request.TradeSessionId);
        const string sql = """
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
            WHERE ts.id = @TradeSessionId;
            """;
        CommandDefinition command = new(sql, parameters, cancellationToken: cancellationToken);
        TradeSessionRow? tradeSessionRow = await connection.QuerySingleOrDefaultAsync<TradeSessionRow>(command);
        if (tradeSessionRow is null)
        {
            return Result.Failure<TradeSessionDetailResponse>(TradeErrors.SessionNotFound);
        }
        BasicUserResponse? confirmedByOfferer = tradeSessionRow.ConfirmedByOfferUserId.HasValue
            ? (await userRepository.FindAsync(u => u.Id == tradeSessionRow.ConfirmedByOfferUserId.Value, cancellationToken))
            .Select(u => new BasicUserResponse(u.Id, u.FirstName, u.LastName, u.AvatarUrl)).SingleOrDefault()
            : null;
        BasicUserResponse? confirmedByRequester = tradeSessionRow.ConfirmedByRequestUserId.HasValue
            ? (await userRepository.FindAsync(u => u.Id == tradeSessionRow.ConfirmedByRequestUserId.Value, cancellationToken))
            .Select(u => new BasicUserResponse(u.Id, u.FirstName, u.LastName, u.AvatarUrl)).SingleOrDefault()
            : null;
        var tradeSessionResponse = new TradeSessionResponse(
            tradeSessionRow.TradeSessionId,
            tradeSessionRow.TradeOfferId,
            tradeSessionRow.TradeRequestId,
            tradeSessionRow.OfferHouseholdId,
            tradeSessionRow.OfferHouseholdName,
            tradeSessionRow.RequestHouseholdId,
            tradeSessionRow.RequestHouseholdName,
            tradeSessionRow.Status,
            tradeSessionRow.StartedOn,
            tradeSessionRow.TotalOfferedItems,
            tradeSessionRow.TotalRequestedItems,
            confirmedByOfferer,
            confirmedByRequester
        );
        const string messagesSql = """
            SELECT
              tsm.id                                 AS TradeSessionMessageId,
              tsm.user_id                            AS UserId,
              tsm.message_text                       AS MessageText,
              tsm.sent_on                            AS SentOn,
              u.id 								     AS UserId,
              u.first_name                           AS FirstName,
              u.last_name                            AS LastName,
              u.avatar_url                           AS AvatarUrl
            FROM trade_session_messages tsm
            JOIN users u ON u.id = tsm.user_id
            WHERE tsm.trade_session_id = @TradeSessionId
            ORDER BY tsm.sent_on DESC;
            """;
        CommandDefinition messagesCommand = new(messagesSql, parameters, cancellationToken: cancellationToken);
        IEnumerable<TradeSessionMessageResponse> messages = await connection.QueryAsync<TradeSessionMessageResponse, BasicUserResponse, TradeSessionMessageResponse>(
            messagesCommand,
            (message, user) =>
            {
                message = message with { User = user };
                return message;
            },
            splitOn: "UserId"
        );
        const string itemsSql = """
            SELECT
              tsi.id                                 AS TradeItemId,
              tsi.food_item_id                       AS FoodItemId,
              fi.name                                 AS Name,
              fr.name                                 AS OriginalName,
              fi.image_url                           AS ImageUrl,
              fr.food_group                          AS FoodGroup,
              tsi.quantity                           AS Quantity,
              u.abbreviation                        AS UnitAbbreviation,
              tsi.unit_id                             AS UnitId,
              fi.expiration_date                     AS ExpirationDate,
              tsi.from                               AS From
            FROM trade_session_items tsi
            JOIN food_items fi ON fi.id = tsi.food_item_id
            JOIN food_references fr ON fr.id = fi.food_reference_id
            JOIN units u ON u.id = tsi.unit_id
            WHERE tsi.session_id = @TradeSessionId;
            """;
        CommandDefinition itemsCommand = new(itemsSql, parameters, cancellationToken: cancellationToken);
        IEnumerable<TradeItemResponse> items = await connection.QueryAsync<TradeItemResponse>(itemsCommand);
        var tradeSessionDetailResponse = new TradeSessionDetailResponse(tradeSessionResponse, messages.AsList(), items.AsList());
        return tradeSessionDetailResponse;
    }
}
