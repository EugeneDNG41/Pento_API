using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Trades.Requests.GetAll;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Requests.GetById;

internal sealed class GetTradeRequestByIdQueryHandler(
    IUserContext userContext,
    ISqlConnectionFactory sqlConnectionFactory
    ) : IQueryHandler<GetTradeRequestByIdQuery, TradeRequestDetailResponse>
{
    public async Task<Result<TradeRequestDetailResponse>> Handle(
        GetTradeRequestByIdQuery query,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId == null)
        {
            return Result.Failure<TradeRequestDetailResponse>(HouseholdErrors.NotInAnyHouseHold);
        }
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string sql = @"
            SELECT 
                tr.id AS TradeRequestId,
                tr.trade_offer_id AS TradeOfferId,
                h1.name AS OfferHouseholdName,
                h2.name AS RequestHouseholdName,
                tr.status AS Status,
                tr.created_on AS CreatedOn,
                tr.updated_on AS UpdatedOn,
                COALESCE(
                    (SELECT COUNT(*) FROM trade_items ti
                     WHERE ti.request_id = tr.id), 0) AS TotalItems
            FROM trade_requests tr
            JOIN trade_offers tof ON tr.trade_offer_id = tof.id
            JOIN households h1 ON tof.household_id = h1.id
            JOIN households h2 ON tr.household_id = h2.id
            WHERE tr.id = @TradeRequestId AND (h1.id = @HouseholdId OR h2.id = @HouseholdId) and tr.is_deleted = false;
        ";
        var parameters = new DynamicParameters();
        parameters.Add("@TradeRequestId", query.TradeRequestId);
        parameters.Add("@HouseholdId", householdId.Value);
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        TradeRequestResponse? tradeRequest = await connection.QueryFirstOrDefaultAsync<TradeRequestResponse>(command);
        if (tradeRequest == null)
        {
            return Result.Failure<TradeRequestDetailResponse>(TradeErrors.RequestNotFound);
        }
        const string itemsSql = """
            SELECT
              ti.id                                 AS TradeItemId,
              ti.food_item_id                       AS FoodItemId,
              fi.name                                 AS Name,
              fr.name                                 AS OriginalName,
              fi.image_url                           AS ImageUrl,
              fr.food_group                          AS FoodGroup,
              ti.quantity                           AS Quantity,
              u.abbreviation                        AS UnitAbbreviation,
              ti.unit_id                             AS UnitId,
              fi.expiration_date                     AS ExpirationDate,
              ti.from                               AS From
            FROM trade_items ti
            JOIN food_items fi ON fi.id = ti.food_item_id
            JOIN food_references fr ON fr.id = fi.food_reference_id
            JOIN units u ON u.id = ti.unit_id
            WHERE ti.request_id = @TradeRequestId;
            """;
        var itemsCommand = new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken);
        IEnumerable<TradeItemResponse> items = await connection.QueryAsync<TradeItemResponse>(itemsCommand);
        var response = new TradeRequestDetailResponse(
            TradeRequest: tradeRequest,
            Items: items.AsList());
        return response;
    }
}
