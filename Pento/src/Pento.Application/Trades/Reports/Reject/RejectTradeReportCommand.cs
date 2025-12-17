using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Reports.Reject;

public sealed record RejectTradeReportCommand(Guid TradeReportId)
    : ICommand;
