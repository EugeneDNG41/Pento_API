using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.TradeItem.Requests.Get;
public sealed record GetTradeRequestsByOfferQuery(Guid OfferId)
    : IQuery<IReadOnlyList<TradeRequestResponse>>;
