using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GiveawayClaims.Accept;

namespace Pento.API.Endpoints.GiveawayClaims.Post;

internal sealed class AcceptGiveawayClaim : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("giveawayclaims/{id:guid}/accept", async (
            Guid id,
            ICommandHandler<AcceptGiveawayClaimCommand, Guid> handler,
            CancellationToken ct
        ) =>
        {
            Domain.Abstractions.Result<Guid> result = await handler.Handle(new AcceptGiveawayClaimCommand(id), ct);

            return result.Match(
                _ => Results.Ok(id),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GiveawayClaims)
        .RequireAuthorization();
    }
}
