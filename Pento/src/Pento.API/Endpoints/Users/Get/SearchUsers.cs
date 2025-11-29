using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.Users.Search;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Get;

internal sealed class SearchUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("admin/users", async (
            string? searchText,
            bool? isDeleted,
            IQueryHandler<SearchUserQuery, PagedList<UserPreview>> handler, CancellationToken cancellationToken,
            int pageNumber = 1,
            int pageSize = 10) =>
        {
            Result<PagedList<UserPreview>> result = await handler.Handle(new SearchUserQuery(searchText, isDeleted, pageNumber, pageSize), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization(Permissions.ManageUsers)
        .WithTags(Tags.Admin);
    }
}
