using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Milestones.GetById;

namespace Pento.Application.Milestones.GetAll;

public sealed record GetAdminMilestonesQuery(string? SearchTerm, bool? IsActive, bool? IsDeleted, GetAdminMilestoneSortBy SortBy, SortOrder SortOrder, int PageNumber, int PageSize) : IQuery<PagedList<AdminMilestoneResponse>>;
public enum GetAdminMilestoneSortBy
{
    Id,
    Name,
    EarnedCount
}
