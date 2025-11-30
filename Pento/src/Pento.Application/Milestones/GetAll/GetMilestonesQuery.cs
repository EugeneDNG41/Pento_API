using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Milestones.GetById;

namespace Pento.Application.Milestones.GetAll;

public sealed record GetMilestonesQuery(string? SearchTerm, bool? IsActive, int PageNumber, int PageSize) : IQuery<PagedList<MilestoneResponse>>;
