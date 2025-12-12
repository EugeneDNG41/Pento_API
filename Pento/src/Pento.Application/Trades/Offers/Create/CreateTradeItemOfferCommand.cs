using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Offers.Create;

public sealed record CreateTradeItemOfferCommand(
    DateTime StartDate,
    DateTime EndDate,
    PickupOption PickupOption,
    List<AddTradeItemDto> Items
) : ICommand<Guid>;
