using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GiveawayClaims.Create;

namespace Pento.API.Endpoints.GiveawayClaims.Post;

internal sealed class CreateGiveawayClaim : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("giveaway/{postId:guid}/claims", async (
            Guid postId,
            Request dto,
            ICommandHandler<CreateGiveawayClaimCommand, Guid> handler,
            CancellationToken ct
        ) =>
        {
            var cmd = new CreateGiveawayClaimCommand(
                postId,
                dto.Message
            );

            Domain.Abstractions.Result<Guid> result = await handler.Handle(cmd, ct);

            return result.Match(
                id => Results.Ok(new { ClaimId = id }),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.GiveawayClaims)
        .RequireAuthorization();
    }

    internal sealed class Request
    {
        public string? Message { get; init; }
    }
}
