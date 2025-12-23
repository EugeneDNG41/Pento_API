using System.Data.Common;
using Dapper;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.Abstractions;
using Pento.Domain.Trades;
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

        #region Filters

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

        #endregion

        #region SQL

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
                tr.id                        AS ReportId,
                tr.trade_session_id          AS TradeSessionId,
                tr.reason                    AS Reason,
                tr.severity                  AS Severity,
                tr.status                    AS Status,
                tr.description               AS Description,
                tr.created_on                AS CreatedOnUtc,

                ru.id                        AS ReporterUserId,
                ru.first_name                AS ReporterFirstName,
                ru.last_name                 AS ReporterLastName,
                ru.avatar_url                AS ReporterAvatarUrl,

                fri.id                       AS FoodItemId,
                fr.name                      AS FoodName,
                fr.image_url                 AS FoodImageUri,
                ti.quantity                  AS Quantity,
                un.abbreviation              AS UnitAbbreviation,

                m.id                         AS MediaId,
                m.media_type                 AS MediaType,
                m.media_uri                  AS MediaUri,

                ts.id                        AS TsId,
                ts.trade_offer_id            AS TradeOfferId,
                ts.trade_request_id          AS TradeRequestId,
                ts.status                    AS TradeSessionStatus,
                ts.started_on                AS StartedOn,

                ho.id                        AS OfferHouseholdId,
                ho.name                      AS OfferHouseholdName,
                hr.id                        AS RequestHouseholdId,
                hr.name                      AS RequestHouseholdName,

                uo.id                        AS OfferConfirmUserId,
                uo.first_name                AS OfferConfirmFirstName,
                uo.last_name                 AS OfferConfirmLastName,
                uo.avatar_url                AS OfferConfirmAvatarUrl,

                ur.id                        AS RequestConfirmUserId,
                ur.first_name                AS RequestConfirmFirstName,
                ur.last_name                 AS RequestConfirmLastName,
                ur.avatar_url                AS RequestConfirmAvatarUrl,

                (
                    SELECT COUNT(*)
                    FROM trade_items tio
                    WHERE tio.offer_id = ts.trade_offer_id
                ) AS TotalOfferedItems,

                (
                    SELECT COUNT(*)
                    FROM trade_items tir
                    WHERE tir.request_id = ts.trade_request_id
                ) AS TotalRequestedItems

            FROM trade_reports tr
            JOIN users ru                       ON ru.id = tr.reporter_user_id
            JOIN trade_sessions ts              ON ts.id = tr.trade_session_id
            JOIN trade_offers o                 ON o.id = ts.trade_offer_id
            JOIN trade_requests r               ON r.id = ts.trade_request_id
            JOIN households ho                  ON ho.id = o.household_id
            JOIN households hr                  ON hr.id = r.household_id

            LEFT JOIN trade_items ti             ON ti.offer_id = o.id
            LEFT JOIN food_items fri             ON fri.id = ti.food_item_id
            LEFT JOIN food_references fr         ON fr.id = fri.food_reference_id
            LEFT JOIN units un                   ON un.id = ti.unit_id
            LEFT JOIN trade_report_medias m      ON m.trade_report_id = tr.id

            LEFT JOIN users uo                   ON uo.id = ts.confirmed_by_offer_user_id
            LEFT JOIN users ur                   ON ur.id = ts.confirmed_by_request_user_id

            {whereClause}
            {orderBy}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        """;

        #endregion

        parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
        parameters.Add("PageSize", request.PageSize);

        using SqlMapper.GridReader multi =
            await connection.QueryMultipleAsync(sql, parameters);

        TradeReportSummaryResponse summary =
            await multi.ReadFirstAsync<TradeReportSummaryResponse>();

        int totalCount =
            await multi.ReadFirstAsync<int>();

        var rows = (await multi.ReadAsync()).ToList();

        var reports = rows
            .Select(MapTradeReport)
            .ToList();

        var pagedReports = PagedList<TradeReportResponse>.Create(
            reports,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return Result.Success(
            new TradeReportPagedResponse(pagedReports, summary)
        );
    }

    #region Mapping Helpers

    private static TradeReportResponse MapTradeReport(dynamic r)
    {
        return new TradeReportResponse(
            r.ReportId,
            r.TradeSessionId,

            r.Reason,
            r.Severity,
            r.Status,
            r.Description,
            r.CreatedOnUtc,

            r.ReporterUserId,
            BuildFullName(r.ReporterFirstName, r.ReporterLastName),
            ToUri(r.ReporterAvatarUrl),

            r.FoodItemId,
            r.FoodName,
            ToUri(r.FoodImageUri),
            r.Quantity,
            r.UnitAbbreviation,

            r.MediaId,
            r.MediaType,
            ToUri(r.MediaUri),

            MapTradeSession(r)
        );
    }

    private static TradeSessionSummaryResponse MapTradeSession(dynamic r)
    {
        return new TradeSessionSummaryResponse(
            r.TsId,
            r.TradeOfferId,
            r.TradeRequestId,

            r.OfferHouseholdId,
            r.OfferHouseholdName,

            r.RequestHouseholdId,
            r.RequestHouseholdName,

            r.TradeSessionStatus.ToString(),
            (DateTime)r.StartedOn,

            Convert.ToInt32(r.TotalOfferedItems),
            Convert.ToInt32(r.TotalRequestedItems),


            MapTradeSessionUser(
                r.OfferConfirmUserId,
                r.OfferConfirmFirstName,
                r.OfferConfirmLastName,
                r.OfferConfirmAvatarUrl
            ),

            MapTradeSessionUser(
                r.RequestConfirmUserId,
                r.RequestConfirmFirstName,
                r.RequestConfirmLastName,
                r.RequestConfirmAvatarUrl
            )
        );
    }

    private static TradeSessionUserResponse? MapTradeSessionUser(
        Guid? userId,
        string? firstName,
        string? lastName,
        string? avatarUrl)
    {
        if (userId is null)
        {
            return null;
        }

        return new TradeSessionUserResponse(
            userId.Value,
            firstName ?? string.Empty,
            lastName ?? string.Empty,
            ToUri(avatarUrl)
        );
    }

    private static Uri? ToUri(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : new Uri(value);

    private static string BuildFullName(string? firstName, string? lastName)
        => string.Join(
            " ",
            new[] { firstName, lastName }
                .Where(x => !string.IsNullOrWhiteSpace(x))
        );

    #endregion
}
