using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.GiveawayPosts.Delete;

namespace Pento.API.Endpoints.GiveawayPosts.Delete;

internal sealed class DeleteGiveawayPost : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("giveawayposts/{id:guid}", async (
            Guid id,
            ICommandHandler<DeleteGiveawayPostCommand, Guid> handler,
            CancellationToken ct
        ) =>
        {
            Domain.Abstractions.Result<Guid> result = await handler.Handle(new DeleteGiveawayPostCommand(id), ct);

            return result.Match(
                _ => Results.Ok(id),
                CustomResults.Problem
            );
        })
         .WithTags(Tags.GiveawayPosts)
        .RequireAuthorization();
    }
}
