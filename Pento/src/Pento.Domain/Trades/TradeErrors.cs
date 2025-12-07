using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public sealed class TradeErrors
{
    public static readonly Error NotFound = Error.NotFound("Trade.NotFound", "Trade not found.");
    public static readonly Error OfferNotFound = Error.NotFound("TradeOffer.NotFound", "Trade Offer not found.");
    public static readonly Error DuplicateRequest = Error.Conflict("TradeRequest.Duplicate", "A trade request for this offer by the user already exists.");
}
