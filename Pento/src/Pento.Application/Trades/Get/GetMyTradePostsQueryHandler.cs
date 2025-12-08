using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;

namespace Pento.Application.Trades.Get;

internal sealed class GetMyTradePostsQueryHandler(
    ISqlConnectionFactory factory,
    IUserContext userContext)
    : IQueryHandler<GetMyTradePostsQuery, IReadOnlyList<TradePostResponse>>
{
    public async Task<Result<IReadOnlyList<TradePostResponse>>> Handle(
        GetMyTradePostsQuery query,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        await using DbConnection connection = await factory.OpenConnectionAsync(cancellationToken);

        const string sql = """
            SELECT
                o.id                    AS OfferId,
                i.id                    AS ItemId,
                i.food_item_id          AS FoodItemId,
                fr.name                 AS FoodName,
                fr.image_url            AS FoodImageUri,
                u.first_name            AS PostedByName,
                u.avatar_url            AS PostedByAvatarUrl,
                i.quantity              AS Quantity,
                un.abbreviation         AS UnitAbbreviation,
                o.start_date            AS StartDate,
                o.end_date              AS EndDate,
                o.pickup_option         AS PickupOption,
                o.user_id               AS PostedBy,
                o.created_on            AS CreatedOnUtc
            FROM trade_offers o
            JOIN trade_items i      ON i.offer_id = o.id
            JOIN food_items fi      ON fi.id = i.food_item_id
            JOIN food_references fr ON fr.id = fi.food_reference_id
            JOIN units un           ON un.id = i.unit_id
            JOIN users u            ON u.id = o.user_id
            WHERE o.user_id = @UserId
              AND o.is_deleted = FALSE
            ORDER BY o.created_on DESC;
        """;

        IEnumerable<TradePostResponse> posts = await connection.QueryAsync<TradePostResponse>(sql, new { UserId = userId });

        return Result.Success<IReadOnlyList<TradePostResponse>>(posts.ToList());
    }
}
