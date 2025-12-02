using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Users.GetAll;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class GetUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/users", async (
            string? searchText,
            bool? isDeleted,
            IQueryHandler<GetUsersQuery, PagedList<UserPreview>> handler, CancellationToken cancellationToken,
            GetUsersSortBy sortBy = GetUsersSortBy.Id,
            SortOrder sortOrder = SortOrder.ASC,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<UserPreview>> result = await handler.Handle(new GetUsersQuery(searchText, isDeleted, sortBy, sortOrder, pageNumber, pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization(Permissions.ManageUsers)
        .WithTags(Tags.Admin);
    }
}
