using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Trades.Reports;

namespace Pento.Application.Trades.Reports.Create;

public sealed record CreateTradeReportCommand(
    Guid TradeSessionId,
    Guid ReportedUserId,
    TradeReportReason Reason,
    FoodSafetyIssueLevel Severity,
    string? Description
) : ICommand<Guid>;
