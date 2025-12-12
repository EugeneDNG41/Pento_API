using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Offers.UpdateItems;

public sealed record UpdateTradeOfferItemsCommand(Guid OfferId, List<UpdateTradeItemDto> Items) : ICommand;
