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
    Uri? MediaUri,
    TradeSessionSummaryResponse TradeSession

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
public sealed record TradeSessionUserResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    Uri? AvatarUrl
);
public sealed record TradeSessionSummaryResponse(
    Guid TradeSessionId,
    Guid TradeOfferId,
    Guid TradeRequestId,

    Guid OfferHouseholdId,
    string OfferHouseholdName,

    Guid RequestHouseholdId,
    string RequestHouseholdName,

    string Status,  
    DateTime StartedOn,

    int TotalOfferedItems,
    int TotalRequestedItems,

    TradeSessionUserResponse? ConfirmedByOfferUser,
    TradeSessionUserResponse? ConfirmedByRequestUser
);

