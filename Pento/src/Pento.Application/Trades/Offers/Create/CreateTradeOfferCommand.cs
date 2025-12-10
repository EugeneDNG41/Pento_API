using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.Create;

public sealed record CreateTradeOfferCommand(
    DateTime StartDate,
    DateTime EndDate,
    PickupOption PickupOption
) : ICommand<Guid>;
