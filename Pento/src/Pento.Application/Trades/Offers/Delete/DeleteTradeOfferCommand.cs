using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.Offers.Delete;

public sealed record DeleteTradeOfferCommand(Guid OfferId) : ICommand;
