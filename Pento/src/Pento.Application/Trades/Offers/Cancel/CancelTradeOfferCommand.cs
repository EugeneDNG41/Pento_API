using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Offers.Cancel;

public sealed record CancelTradeOfferCommand(Guid OfferId) : ICommand;
