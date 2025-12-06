using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.FoodItemLogs.GetSummary;
using Pento.Application.UserActivities.GetUserActivities;
using Pento.Application.UserMilestones.GetMilestones;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetMilestones : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/milestones", async (
            string? searchTerm,
            bool? isAchieved,                       
            IQueryHandler<GetMilestonesQuery, PagedList<UserMilestonePreviewResponse>> handler,
            CancellationToken cancellationToken,
            UserMilestoneSortBy sortBy = UserMilestoneSortBy.Default,
            SortOrder sortOrder = SortOrder.ASC,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            var query = new GetMilestonesQuery(
                searchTerm,
                isAchieved,
                sortBy,
                sortOrder,
                pageNumber,
                pageSize);
            Result<PagedList<UserMilestonePreviewResponse>> result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Users).RequireAuthorization();
    }
}
