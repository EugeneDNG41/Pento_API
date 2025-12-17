using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Trades.Reports.GetAll;
using Pento.Domain.Abstractions;

namespace Pento.Application.Trades.Reports.Get;

internal sealed class GetTradeReportByIdQueryHandler(
    ISqlConnectionFactory factory)
    : IQueryHandler<GetTradeReportByIdQuery, IReadOnlyList<TradeReportResponse>>
{
    public async Task<Result<IReadOnlyList<TradeReportResponse>>> Handle(
        GetTradeReportByIdQuery query,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection =
            await factory.OpenConnectionAsync(cancellationToken);

        const string sql = """
            SELECT
                tr.id                    AS ReportId,
                tr.trade_session_id      AS TradeSessionId,
                tr.reason                AS Reason,
                tr.severity              AS Severity,
                tr.status                AS Status,
                tr.description           AS Description,
                tr.created_on            AS CreatedOnUtc,

                ru.id                    AS ReporterUserId,
                ru.first_name            AS ReporterName,
                ru.avatar_url            AS ReporterAvatarUrl,

                pu.id                    AS ReportedUserId,
                pu.first_name            AS ReportedName,
                pu.avatar_url            AS ReportedAvatarUrl,

                fri.id                   AS FoodItemId,
                fr.name                  AS FoodName,
                fr.image_url             AS FoodImageUri,
                ti.quantity              AS Quantity,
                un.abbreviation          AS UnitAbbreviation,

                m.id                     AS MediaId,
                m.media_type             AS MediaType,
                m.media_uri              AS MediaUri
            FROM trade_reports tr
            JOIN trade_sessions ts          ON ts.id = tr.trade_session_id
            JOIN trade_offers o             ON o.id = ts.trade_offer_id
            JOIN trade_requests r           ON r.id = ts.trade_request_id

            JOIN users ru                   ON ru.id = tr.reporter_user_id
            JOIN users pu                   ON pu.id = tr.reported_user_id

            LEFT JOIN trade_items ti        ON ti.offer_id = o.id
            LEFT JOIN food_items fri        ON fri.id = ti.food_item_id
            LEFT JOIN food_references fr    ON fr.id = fri.food_reference_id
            LEFT JOIN units un              ON un.id = ti.unit_id

            LEFT JOIN trade_report_medias m  ON m.trade_report_id = tr.id

            WHERE tr.id = @TradeReportId
              AND tr.is_deleted = FALSE
            ORDER BY tr.created_on DESC;
        """;

        IEnumerable<TradeReportResponse> rows =
            await connection.QueryAsync<TradeReportResponse>(
                sql,
                new { query.TradeReportId });

        return Result.Success<IReadOnlyList<TradeReportResponse>>(rows.ToList());
    }
}
