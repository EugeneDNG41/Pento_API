using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Trades.TradeItem.Requests.Create;
public sealed record CreateTradeItemRequestCommand(
    Guid TradeOfferId,
    List<CreateTradeItemRequestDto> Items
) : ICommand<Guid>;
public sealed record CreateTradeItemRequestDto(
    Guid FoodItemId,
    decimal Quantity,
    Guid UnitId
);




