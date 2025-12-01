using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.UserMilestones.GetCurrentMilestones;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetCurrentMilestones : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/milestones", async (
            string? searchTerm,
            bool? isAchieved,
            UserMilestoneSortBy? sortBy,            
            IQueryHandler<GetCurrentMilestonesQuery, PagedList<CurrentUserMilestonesResponse>> handler,
            CancellationToken cancellationToken,
            SortOrder sortOrder = SortOrder.ASC,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var query = new GetCurrentMilestonesQuery(
                searchTerm,
                isAchieved,
                sortBy,
                sortOrder,
                pageNumber,
                pageSize);
            Result<PagedList<CurrentUserMilestonesResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Users).RequireAuthorization();
    }
}
