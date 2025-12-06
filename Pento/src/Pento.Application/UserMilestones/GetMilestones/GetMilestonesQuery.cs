using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.UserMilestones.GetMilestones;

public sealed record GetMilestonesQuery(string? SearchTerm, bool? IsAchieved, UserMilestoneSortBy SortBy, SortOrder SortOrder, int PageNumber, int PageSize) : IQuery<PagedList<UserMilestonePreviewResponse>>;
public enum UserMilestoneSortBy
{
    Default = 1,
    Name = 2,
    AchievedOn = 3,
    Progress = 4
}
