using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Reports.GetAll;

namespace Pento.Application.Trades.Reports.Get;

public sealed record GetTradeReportByIdQuery(Guid TradeReportId)
    : IQuery<IReadOnlyList<TradeReportResponse>>;
