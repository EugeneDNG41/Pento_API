using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Trades.TradeItem.Requests.Get;
using Pento.Domain.Abstractions;

namespace Pento.Application.Trades.Requests.Get;

internal sealed class GetTradeRequestsByOfferQueryHandler(ISqlConnectionFactory factory)
    : IQueryHandler<GetTradeRequestsByOfferQuery, IReadOnlyList<TradeRequestResponse>>
{
    public async Task<Result<IReadOnlyList<TradeRequestResponse>>> Handle(
        GetTradeRequestsByOfferQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await factory.OpenConnectionAsync(cancellationToken);

        const string sql = """
            SELECT
                r.id                AS RequestId,
                r.user_id           AS UserId,
                u.first_name        AS FirstName,
                u.avatar_url        AS AvatarUrl,
                r.status            AS Status,
                r.created_on        AS CreatedOn,

                i.id                AS ItemId,
                i.food_item_id      AS FoodItemId,
                fr.name             AS FoodName,
                fr.image_url        AS FoodImageUri,
                i.quantity          AS Quantity,
                un.abbreviation     AS UnitAbbreviation
            FROM trade_requests r
            LEFT JOIN users u ON r.user_id = u.id
            LEFT JOIN trade_item_requests i ON r.id = i.request_id
            LEFT JOIN food_items fi ON i.food_item_id = fi.id
            LEFT JOIN food_references fr ON fi.food_reference_id = fr.id
            LEFT JOIN units un ON i.unit_id = un.id
            WHERE r.trade_offer_id = @OfferId
            ORDER BY r.created_on DESC, i.id;
        """;

        IEnumerable<TradeRequestJoinedRow> rows = await connection.QueryAsync<TradeRequestJoinedRow>(
            new CommandDefinition(sql, request, cancellationToken: cancellationToken)
        );

        var grouped = rows
            .GroupBy(r => new {
                r.RequestId,
                r.UserId,
                r.FirstName,
                r.AvatarUrl,
                r.Status,
                r.CreatedOn
            })
            .Select(g =>
                new TradeRequestResponse(
                    g.Key.RequestId,
                    g.Key.UserId,
                    g.Key.FirstName,
                    g.Key.AvatarUrl,
                    g.Key.Status,
                    g.Key.CreatedOn,
                    g
                        .Where(x => x.ItemId != Guid.Empty)
                        .Select(x =>
                            new TradeRequestItemResponse(
                                x.ItemId,
                                x.FoodItemId,
                                x.FoodName,
                                x.FoodImageUri,
                                x.Quantity,
                                x.UnitAbbreviation
                            )
                        )
                        .ToList()
                )
            )
            .ToList();

        return grouped;
    }
}
