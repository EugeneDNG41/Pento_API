using Pento.Application.Abstractions.Messaging;
using Pento.Application.Trades.Sessions.GetById;

namespace Pento.Application.Trades.Offers.AddItems;

public sealed record AddTradeOfferItemsCommand(Guid OfferId, List<AddTradeItemDto> Items) : ICommand<IReadOnlyList<TradeItemResponse>>;
