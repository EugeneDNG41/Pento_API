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
                )                             AS TotalOfferedItems,

                (
                    SELECT COUNT(*)
                    FROM trade_items tir
                    WHERE tir.request_id = ts.trade_request_id
                )                             AS TotalRequestedItems

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

        parameters.Add("Offset", (request.PageNumber - 1) * request.PageSize);
        parameters.Add("PageSize", request.PageSize);

        using SqlMapper.GridReader multi =
            await connection.QueryMultipleAsync(sql, parameters);

        TradeReportSummaryResponse summary =
            await multi.ReadFirstAsync<TradeReportSummaryResponse>();

        int totalCount =
            await multi.ReadFirstAsync<int>();

        var rows = (await multi.ReadAsync()).ToList();

        var reports = rows.Select(r =>
        {
            // Reporter name (an toàn null)
            string reporterName = string.Join(
                " ",
                new[] { r.ReporterFirstName, r.ReporterLastName }
                    .Where(x => !string.IsNullOrWhiteSpace(x))
            );

            Uri? reporterAvatar = string.IsNullOrWhiteSpace(r.ReporterAvatarUrl)
                ? null
                : new Uri(r.ReporterAvatarUrl);

            Uri? foodImage = string.IsNullOrWhiteSpace(r.FoodImageUri)
                ? null
                : new Uri(r.FoodImageUri);

            Uri? mediaUri = string.IsNullOrWhiteSpace(r.MediaUri)
                ? null
                : new Uri(r.MediaUri);

            Uri? offerConfirmAvatar = string.IsNullOrWhiteSpace(r.OfferConfirmAvatarUrl)
                ? null
                : new Uri(r.OfferConfirmAvatarUrl);

            TradeSessionUserResponse? confirmedByOfferUser =
                r.OfferConfirmUserId is null
                    ? null
                    : new TradeSessionUserResponse(
                        r.OfferConfirmUserId,
                        r.OfferConfirmFirstName ?? string.Empty,
                        r.OfferConfirmLastName ?? string.Empty,
                        offerConfirmAvatar
                    );

            Uri? requestConfirmAvatar = string.IsNullOrWhiteSpace(r.RequestConfirmAvatarUrl)
                ? null
                : new Uri(r.RequestConfirmAvatarUrl);

            TradeSessionUserResponse? confirmedByRequestUser =
                r.RequestConfirmUserId is null
                    ? null
                    : new TradeSessionUserResponse(
                        r.RequestConfirmUserId,
                        r.RequestConfirmFirstName ?? string.Empty,
                        r.RequestConfirmLastName ?? string.Empty,
                        requestConfirmAvatar
                    );

            return new TradeReportResponse(
                ReportId: r.ReportId,
                TradeSessionId: r.TradeSessionId,

                Reason: r.Reason,
                Severity: r.Severity,
                Status: r.Status,
                Description: r.Description,
                CreatedOnUtc: r.CreatedOnUtc,

                ReporterUserId: r.ReporterUserId,
                ReporterName: reporterName,
                ReporterAvatarUrl: reporterAvatar,

                FoodItemId: r.FoodItemId,
                FoodName: r.FoodName,
                FoodImageUri: foodImage,
                Quantity: r.Quantity,
                UnitAbbreviation: r.UnitAbbreviation,

                MediaId: r.MediaId,
                MediaType: r.MediaType,
                MediaUri: mediaUri,

                TradeSession: new TradeSessionSummaryResponse(
                    TradeSessionId: r.TsId,
                    TradeOfferId: r.TradeOfferId,
                    TradeRequestId: r.TradeRequestId,

                    OfferHouseholdId: r.OfferHouseholdId,
                    OfferHouseholdName: r.OfferHouseholdName,

                    RequestHouseholdId: r.RequestHouseholdId,
                    RequestHouseholdName: r.RequestHouseholdName,

                    Status: r.TradeSessionStatus,
                    StartedOn: r.StartedOn,

                    TotalOfferedItems: r.TotalOfferedItems,
                    TotalRequestedItems: r.TotalRequestedItems,

                    ConfirmedByOfferUser: confirmedByOfferUser,
                    ConfirmedByRequestUser: confirmedByRequestUser
                )
            );
        }).ToList();



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
}
