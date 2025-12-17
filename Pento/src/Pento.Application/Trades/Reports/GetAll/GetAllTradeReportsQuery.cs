using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Reports.GetAll;

public sealed record GetAllTradeReportsQuery
    : IQuery<IReadOnlyList<TradeReportResponse>>;
