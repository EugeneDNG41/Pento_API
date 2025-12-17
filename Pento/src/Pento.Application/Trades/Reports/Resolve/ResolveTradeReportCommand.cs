using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Reports.Resolve;

public sealed record ResolveTradeReportCommand(Guid TradeReportId)
    : ICommand;
