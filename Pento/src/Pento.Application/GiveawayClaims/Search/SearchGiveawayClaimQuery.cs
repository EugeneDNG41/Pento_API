using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Domain.GiveawayClaims;

namespace Pento.Application.GiveawayClaims.Search;

public sealed record SearchGiveawayClaimQuery(
    Guid? GiveawayPostId,
    ClaimStatus? Status,
    string? SearchText,
    string? SortBy,         
    bool Descending,         
    int PageNumber,
    int PageSize
) : IQuery<PagedList<GiveawayClaimPreview>>;

