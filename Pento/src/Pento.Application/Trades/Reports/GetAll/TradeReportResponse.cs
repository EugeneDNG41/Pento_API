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

    Guid ReportedUserId,
    string ReportedName,
    Uri? ReportedAvatarUrl,

    Guid? FoodItemId,
    string? FoodName,
    Uri? FoodImageUri,
    decimal? Quantity,
    string? UnitAbbreviation,

    Guid? MediaId,
    string? MediaType,
    Uri? MediaUri
);
