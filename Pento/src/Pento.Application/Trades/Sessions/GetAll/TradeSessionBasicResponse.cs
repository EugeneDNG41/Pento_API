using Pento.Domain.Trades;

namespace Pento.Application.Trades.Sessions.GetAll;
public sealed record TradeSessionBasicResponse
   (
       Guid TradeSessionId,
       TradeSessionStatus Status,
       DateTime StartedOn,
       int TotalOfferedItems,
       int TotalRequestedItems,
       IReadOnlyList<Uri>? AvatarUrls
   );
