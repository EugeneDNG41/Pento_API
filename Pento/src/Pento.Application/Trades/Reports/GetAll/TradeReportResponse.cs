using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.Trades.Reports.GetAll;

public sealed record TradeReportResponse(
    // ===== Trade Report =====
    Guid ReportId,
    Guid TradeSessionId,
    string Reason,
    string Severity,
    string Status,
    string? Description,
    DateTime CreatedOnUtc,

    // ===== Reporter =====
    Guid ReporterUserId,
    string ReporterName,
    Uri? ReporterAvatarUrl,

    // ===== Food =====
    Guid? FoodItemId,
    string? FoodName,
    Uri? FoodImageUri,
    decimal? Quantity,
    string? UnitAbbreviation,

    // ===== Media =====
    Guid? MediaId,
    string? MediaType,
    Uri? MediaUri,

    // ===== Trade Session (NEW) =====
    string? TradeSessionStatus,
    DateTime? TradeSessionStartedOn,
    string? OfferHouseholdName,
    string? RequestHouseholdName
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
