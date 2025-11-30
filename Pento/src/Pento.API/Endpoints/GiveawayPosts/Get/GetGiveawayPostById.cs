using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GiveawayPosts.Get;

namespace Pento.API.Endpoints.GiveawayPosts.Get;

internal sealed class GetGiveawayPostById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("giveawayposts/{id:guid}", async (
            Guid id,
            IQueryHandler<GetGiveawayPostByIdQuery, GiveawayPostDetailResponse> handler,
            CancellationToken ct
        ) =>
        {
            Domain.Abstractions.Result<GiveawayPostDetailResponse> result = await handler.Handle(
                new GetGiveawayPostByIdQuery(id),
                ct
            );

            return result.Match(
                Results.Ok,
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GiveawayPosts)
        .RequireAuthorization();
    }
}
