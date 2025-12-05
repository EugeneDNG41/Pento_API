using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.FoodItemLogs.GetSummary;
using Pento.Application.Households.Get;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Get;

internal sealed class GetFoodItemLogSummary : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/food-item-logs/summary", async (
            Guid? householdId,
            DateTime? fromDate,
            DateTime? toDate,
            Guid? weightUnitId,
            Guid? volumeUnitId,
            bool? isDeleted,
            IQueryHandler<GetAdminFoodItemLogSummaryQuery, FoodSummary> handler,
            CancellationToken cancellationToken) =>
        {
            Result<FoodSummary> result = await handler.Handle(new GetAdminFoodItemLogSummaryQuery(
                householdId,
                fromDate?.ToUniversalTime(),
                toDate?.ToUniversalTime(),
                weightUnitId,
                volumeUnitId,
                isDeleted), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .RequireAuthorization(Permissions.ManageHouseholds);
    }
}
internal sealed class GetHouseholds : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/households", async (
            string? searchText,
            int? fromMember,
            int? toMember,
            bool? isDeleted,
            GetHouseholdsSortBy? sortBy,
            SortOrder? sortOrder,
            IQueryHandler<GetHouseholdsQuery, PagedList<HouseholdPreview>> handler,
            CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<HouseholdPreview>> result = await handler.Handle(
                new GetHouseholdsQuery(
                    searchText,
                    isDeleted,
                    fromMember,
                    toMember,
                    sortBy,
                    sortOrder,
                    pageNumber,
                    pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Admin);
    }
}
