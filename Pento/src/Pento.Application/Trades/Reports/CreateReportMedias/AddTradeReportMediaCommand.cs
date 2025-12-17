using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Reports.CreateReportMedias;

public sealed record AddTradeReportMediaCommand(
    Guid TradeReportId,
    TradeReportMedia.TradeReportMediaType MediaType,
    IFormFile File
) : ICommand<Guid>;
