using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.Trades.Get;
public sealed record TradePostResponse(
    Guid OfferId,
    Guid ItemId,
    Guid FoodItemId,
    string FoodName,
    Uri? FoodImageUri,
    decimal Quantity,
    string UnitAbbreviation,
    DateTime StartDate,
    DateTime EndDate,
    string PickupOption,
    Guid PostedBy,
    DateTime CreatedOnUtc
);
