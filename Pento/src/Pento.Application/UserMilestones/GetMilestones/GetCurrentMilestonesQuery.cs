using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;

namespace Pento.Application.UserMilestones.GetMilestones;

public sealed record GetCurrentMilestonesQuery(string? SearchTerm, bool? IsAchieved, UserMilestoneSortBy SortBy, SortOrder SortOrder, int PageNumber, int PageSize) : IQuery<PagedList<CurrentUserMilestonesResponse>>;
public enum UserMilestoneSortBy
{
    Default = 1,
    Name = 2,
    AchievedOn = 3,
    Progress = 4
}
