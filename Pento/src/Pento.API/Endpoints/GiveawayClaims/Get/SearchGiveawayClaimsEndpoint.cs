using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Pagination;
using Pento.Application.GiveawayClaims.Get;
using Pento.Application.GiveawayClaims.Search;
using Pento.Application.Giveaways.Claims.Get;
using Pento.Application.Giveaways.Claims.Search;
using Pento.Domain.GiveawayClaims;

namespace Pento.API.Endpoints.Giveaways.Claims;

internal sealed class SearchGiveawayClaimsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("giveawayclaims/search", async (
            Guid? giveawayPostId,
            ClaimStatus? status,
            string? searchText,
            string? sortBy,  
            bool descending,
            int pageNumber,
            int pageSize,
            IQueryHandler<SearchGiveawayClaimQuery, PagedList<GiveawayClaimPreview>> handler,
            CancellationToken ct
        ) =>
        {
            var query = new SearchGiveawayClaimQuery(
                giveawayPostId,
                status,
                searchText,
                sortBy,
                descending,
                pageNumber,
                pageSize
            );

            Domain.Abstractions.Result<PagedList<GiveawayClaimPreview>> result = await handler.Handle(query, ct);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.GiveawayClaims)
        .WithDescription("sort by: post, date")
        .RequireAuthorization();

        app.MapGet("giveawayclaims/{id:guid}", async (
            Guid id,
            IQueryHandler<GetGiveawayClaimQuery, GiveawayClaimDetailResponse> handler,
            CancellationToken ct
        ) =>
        {
            Domain.Abstractions.Result<GiveawayClaimDetailResponse> result = await handler.Handle(new GetGiveawayClaimQuery(id), ct);

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GiveawayClaims)
        .RequireAuthorization();
    }
}
