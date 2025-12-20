using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.Trades.Reports.GetAll;

public sealed record TradeReportResponse(
    Guid ReportId,
    Guid TradeSessionId,
    string Reason,
    string Severity,
    string Status,
    string? Description,
    DateTime CreatedOnUtc,

    Guid ReporterUserId,
    string ReporterName,
    Uri? ReporterAvatarUrl,

    Guid? FoodItemId,
    string? FoodName,
    Uri? FoodImageUri,
    decimal? Quantity,
    string? UnitAbbreviation,

    Guid? MediaId,
    string? MediaType,
    Uri? MediaUri
);
public sealed record TradeReportSummaryResponse(
    long TotalReports,
    long PendingReports,
    long UrgentReports,
    long ResolvedReports
);
public sealed record TradeReportPagedResponse(
    PagedList<TradeReportResponse> Reports,
    TradeReportSummaryResponse Summary
);
