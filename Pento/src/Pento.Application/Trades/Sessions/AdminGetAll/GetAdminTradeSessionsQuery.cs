using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.Trades.Sessions.GetAll;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Sessions.AdminGetAll;

public sealed record GetAdminTradeSessionsQuery(
    Guid? OfferId,
    Guid? RequestId,
    TradeSessionStatus? Status,
    SortOrder? SortOrder,
    int PageNumber,
    int PageSize) : IQuery<PagedList<TradeSessionBasicResponse>>;
