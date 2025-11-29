using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.GiveawayClaims.Search;
using Pento.Domain.GiveawayClaims;

namespace Pento.Application.Giveaways.Claims.Search;

public sealed record SearchGiveawayClaimQuery(
    Guid? GiveawayPostId,
    ClaimStatus? Status,
    string? SearchText,
    string? SortBy,         
    bool Descending,         
    int PageNumber,
    int PageSize
) : IQuery<PagedList<GiveawayClaimPreview>>;

