using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Milestones.GetAll;
using Pento.Application.Milestones.GetById;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Milestone.Get;

internal sealed class GetMilestones : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/milestones", async (
            string? searchText,
            bool? isActive,
            bool? isDeleted,            
            IQueryHandler<GetAdminMilestonesQuery, PagedList<AdminMilestoneResponse>> handler,           
            CancellationToken cancellationToken,
            GetAdminMilestoneSortBy sortBy = GetAdminMilestoneSortBy.Id,
            SortOrder order = SortOrder.ASC,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<AdminMilestoneResponse>> result = await handler.Handle(new GetAdminMilestonesQuery(
                searchText,
                isActive,
                isDeleted,
                sortBy,
                order,
                pageNumber,
                pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageMilestones);
    }
}
