using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades.Reports;

namespace Pento.Application.Trades.Reports.GetAll;

internal sealed class GetAllTradeReportsQueryHandler(
    ISqlConnectionFactory factory)
    : IQueryHandler<GetAllTradeReportsQuery, TradeReportPagedResponse>
{
    public async Task<Result<TradeReportPagedResponse>> Handle(
        GetAllTradeReportsQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection =
            await factory.OpenConnectionAsync(cancellationToken);

        var filters = new List<string>
        {
            "tr.is_deleted = FALSE"
        };

        var parameters = new DynamicParameters();

        if (request.Status is not null)
        {
            filters.Add("tr.status = @Status");
            parameters.Add("Status", request.Status.ToString());
        }

        if (request.Severity is not null)
        {
            filters.Add("tr.severity = @Severity");
            parameters.Add("Severity", request.Severity.ToString());
        }

        if (request.Reason is not null)
        {
            filters.Add("tr.reason = @Reason");
            parameters.Add("Reason", request.Reason.ToString());
        }


        string whereClause = "WHERE " + string.Join(" AND ", filters);

        string orderBy = request.Sort switch
        {
            TradeReportSort.Oldest => "ORDER BY tr.created_on ASC",
            _ => "ORDER BY tr.created_on DESC"
        };

        string sql = $"""
            -- 1️⃣ Summary
            SELECT
                COUNT(*) AS TotalReports,
                COUNT(*) FILTER (WHERE tr.status = '{TradeReportStatus.Pending}') AS PendingReports,
                COUNT(*) FILTER (WHERE tr.severity = '{FoodSafetyIssueLevel.Serious}') AS UrgentReports,
                COUNT(*) FILTER (WHERE tr.status = '{TradeReportStatus.Resolved}') AS ResolvedReports
            FROM trade_reports tr
            {whereClause};

            -- 2️⃣ Total count
            SELECT COUNT(*)
            FROM trade_reports tr
            {whereClause};

            -- 3️⃣ Paged data
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

                fri.id                   AS FoodItemId,
                fr.name                  AS FoodName,
                fr.image_url             AS FoodImageUri,
                ti.quantity              AS Quantity,
                un.abbreviation          AS UnitAbbreviation,

                m.id                     AS MediaId,
                m.media_type             AS MediaType,
                m.media_uri              AS MediaUri
            FROM trade_reports tr
            JOIN users ru                   ON ru.id = tr.reporter_user_id
            LEFT JOIN trade_sessions ts     ON ts.id = tr.trade_session_id
            LEFT JOIN trade_offers o        ON o.id = ts.trade_offer_id
            LEFT JOIN trade_items ti        ON ti.offer_id = o.id
            LEFT JOIN food_items fri        ON fri.id = ti.food_item_id
            LEFT JOIN food_references fr    ON fr.id = fri.food_reference_id
            LEFT JOIN units un              ON un.id = ti.unit_id
            LEFT JOIN trade_report_medias m ON m.trade_report_id = tr.id
            {whereClause}
            {orderBy}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        """;

        parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
        parameters.Add("PageSize", request.PageSize);

        using SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters);

        TradeReportSummaryResponse summary =
            await multi.ReadFirstAsync<TradeReportSummaryResponse>();

        int totalCount =
            await multi.ReadFirstAsync<int>();

        var items =
            (await multi.ReadAsync<TradeReportResponse>()).ToList();

        var pagedReports = PagedList<TradeReportResponse>.Create(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return Result.Success(
            new TradeReportPagedResponse(pagedReports, summary)
        );
    }
}
