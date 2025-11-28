using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Domain.GiveawayPosts;

namespace Pento.Application.GiveawayPosts.Search;
public sealed record SearchGiveawayPostQuery(
    string? SearchText,
    GiveawayStatus? Status,
    PickupOption? PickupOption,
    int PageNumber,
    int PageSize
) : IQuery<PagedList<GiveawayPostPreview>>;

