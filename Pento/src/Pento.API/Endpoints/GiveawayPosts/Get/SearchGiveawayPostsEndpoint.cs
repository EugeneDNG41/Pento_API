using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.GiveawayPosts.Search;
using Pento.Domain.GiveawayPosts;

namespace Pento.API.Endpoints.GiveawayPosts.Get;

internal sealed class SearchGiveawayPostsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("giveawayposts", async (
            string? searchText,
            GiveawayStatus? status,
            PickupOption? pickupOption,
            int pageNumber,
            int pageSize,
            IQueryHandler<SearchGiveawayPostQuery, PagedList<GiveawayPostPreview>> handler,
            CancellationToken ct
        ) =>
        {
            var query = new SearchGiveawayPostQuery(
                searchText,
                status,
                pickupOption,
                pageNumber,
                pageSize
            );

            Domain.Abstractions.Result<PagedList<GiveawayPostPreview>> result = await handler.Handle(query, ct);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GiveawayPosts)
        .RequireAuthorization();
    }
}
