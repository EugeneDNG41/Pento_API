using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.GetById;

namespace Pento.Application.Trades.Requests.AddItems;

public sealed record AddTradeRequestItemsCommand(Guid RequestId, List<AddTradeItemDto> Items) : ICommand<IReadOnlyList<TradeItemResponse>>;
