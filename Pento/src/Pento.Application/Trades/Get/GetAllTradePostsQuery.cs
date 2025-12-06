using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;

namespace Pento.Application.Trades.Get;
public sealed record GetAllTradePostsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    string? Sort = "newest"
) : IQuery<PagedList<TradePostResponse>>;
