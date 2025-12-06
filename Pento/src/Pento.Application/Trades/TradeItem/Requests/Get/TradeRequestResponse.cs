using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.Trades.TradeItem.Requests.Get;
public sealed record TradeRequestResponse(
    Guid RequestId,
    Guid UserId,
    string FirstName,
    Uri? AvatarUrl,
    string Status,
    DateTime CreatedOn,
    IReadOnlyList<TradeRequestItemResponse> Items
);
public sealed record TradeRequestItemResponse(
    Guid ItemId,
    Guid FoodItemId,
    string FoodName,
    Uri? FoodImageUri,
    decimal Quantity,
    string UnitAbbreviation
);
internal sealed record TradeRequestJoinedRow(
    Guid RequestId,
    Guid UserId,
    string FirstName,
    Uri? AvatarUrl,
    string Status,
    DateTime CreatedOn,
    Guid ItemId,
    Guid FoodItemId,
    string FoodName,
    Uri? FoodImageUri,
    decimal Quantity,
    string UnitAbbreviation
);

