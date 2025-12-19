using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Trades.Requests.AdminGetAll;
using Pento.Application.Trades.Sessions.GetById;
using Pento.Application.Users.GetAll;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.AdminGetById;

internal sealed class GetAdminTradeOfferByIdQueryHandler(
    ISqlConnectionFactory sqlConnectionFactory
    ) : IQueryHandler<GetAdminTradeOfferByIdQuery, TradeOfferDetailAdminResponse>
{
    public async Task<Result<TradeOfferDetailAdminResponse>> Handle(
        GetAdminTradeOfferByIdQuery query,
        CancellationToken cancellationToken)
    {
        using DbConnection connection = await sqlConnectionFactory.OpenConnectionAsync(cancellationToken);
        string sql = @"
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
            WHERE tof.id = @TradeOfferId;
        ";
        var parameters = new DynamicParameters();
        parameters.Add("@TradeOfferId", query.TradeOfferId);
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        TradeOfferAdminRow? row = await connection.QueryFirstOrDefaultAsync<TradeOfferAdminRow>(command);
        if (row == null)
        {
            return Result.Failure<TradeOfferDetailAdminResponse>(TradeErrors.RequestNotFound);
        }
        var tradeOffer = new TradeOfferAdminResponse
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
        };
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
            WHERE ti.offer_id = @TradeOfferId;
            """;
        var itemsCommand = new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken);
        IEnumerable<TradeItemResponse> items = await connection.QueryAsync<TradeItemResponse>(itemsCommand);
        var response = new TradeOfferDetailAdminResponse(
            TradeOffer: tradeOffer,
            Items: items.AsList());
        return response;
    }
}
