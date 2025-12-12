using Pento.Domain.Abstractions;

namespace Pento.Domain.Trades;

public sealed class TradeErrors
{
    public static readonly Error NotFound = Error.NotFound("Trade.NotFound", "Trade not found.");
    public static readonly Error OfferNotFound = Error.NotFound("TradeOffer.NotFound", "Trade Offer not found.");
    public static readonly Error RequestNotFound = Error.NotFound("TradeRequest.NotFound", "Trade Request not found.");
    public static readonly Error SessionNotFound = Error.NotFound("TradeSession.NotFound", "Trade Session not found.");
    public static readonly Error DuplicateRequest = Error.Conflict("TradeRequest.Duplicate", "A trade request for this offer by the user already exists.");
    public static readonly Error InvalidOfferState = Error.Conflict("TradeOffer.InvalidState", "Trade offer must be open to accept requests.");
    public static readonly Error InvalidRequestState = Error.Conflict("TradeRequest.InvalidState", "Trade request must be pending to accept or reject.");
    public static readonly Error InvalidSessionState = Error.Conflict("TradeSession.InvalidState", "Trade session must be ongoing to continue.");
    public static readonly Error OfferForbiddenAccess = Error.Forbidden("TradeOffer.Forbidden", "You do not have permission to access this trade offer.");
    public static readonly Error RequestForbiddenAccess = Error.Forbidden("TradeRequest.Forbidden", "You do not have permission to access this trade request.");
    public static readonly Error SessionForbiddenAccess = Error.Forbidden("TradeSession.Forbidden", "You do not have permission to access this trade session.");
    public static readonly Error ItemForbiddenAccess = Error.Forbidden("TradeItem.Forbidden", "You do not have permission to access this trade item.");
    public static readonly Error MessageNotFound = Error.NotFound("TradeMessage.NotFound", "Trade message not found.");
    public static readonly Error CannotTradeWithinHousehold = Error.Conflict("Trade.CannotTradeWithinHousehold", "Cannot trade with members of their own household.");
    public static readonly Error CannotTradeWithSelf = Error.Conflict("Trade.CannotTradeWithSelf", "Cannot trade with yourself.");
    public static readonly Error DuplicateTradeItems = Error.Conflict("Trade.DuplicateItems", "Trade contains duplicate items."); //business rule
    public static readonly Error ExceedsMaxTradeItems = Error.Conflict("Trade.ExceedsMaxItems", "Trade exceeds maximum allowed items."); //business rule
    public static readonly Error AlreadyOngoingSession = Error.Conflict("Trade.AlreadyOngoingSession", "An ongoing trade session already exists between the involved offer and request.");
}
