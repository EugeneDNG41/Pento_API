using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Trades.Reports;

namespace Pento.Application.Trades.Reports.GetAll;

public sealed record GetAllTradeReportsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    TradeReportStatus? Status = null,
    FoodSafetyIssueLevel? Severity = null,
    TradeReportReason? Reason = null,
    TradeReportSort Sort = TradeReportSort.Newest
) : IQuery<TradeReportPagedResponse>;

public enum TradeReportSort
{
    Newest = 1,
    Oldest = 2
}
