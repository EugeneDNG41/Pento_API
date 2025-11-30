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
        app.MapGet("milestones", async (
            string? searchText,
            bool? isActive,
            IQueryHandler<GetMilestonesQuery, PagedList<MilestoneResponse>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<MilestoneResponse>> result = await handler.Handle(new GetMilestonesQuery(
                searchText,
                isActive,
                pageNumber,
                pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Milestones);
    }
}
