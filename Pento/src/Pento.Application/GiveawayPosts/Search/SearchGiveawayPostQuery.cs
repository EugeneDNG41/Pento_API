using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.GiveawayPosts;

namespace Pento.Application.GiveawayPosts.Search;

public sealed record SearchGiveawayPostQuery(
    string? SearchText,
    GiveawayStatus? Status,
    PickupOption? PickupOption,
    int PageNumber,
    int PageSize
) : IQuery<PagedList<GiveawayPostPreview>>;

