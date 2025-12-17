using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades.Reports;

public static class TradeReportMediaErrors
{
    public static readonly Error ReportNotFound =
        Error.NotFound("TradeReportMedia.ReportNotFound", "Trade report not found.");

    public static readonly Error Forbidden =
        Error.Forbidden("TradeReportMedia.Forbidden", "You are not allowed to add media to this report.");

    public static readonly Error UploadFailed =
        Error.Failure("TradeReportMedia.UploadFailed", "Failed to upload report media.");
}
