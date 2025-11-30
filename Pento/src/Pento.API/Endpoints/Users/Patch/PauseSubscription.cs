using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.UserSubscriptions.PauseSubscription;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Patch;

internal sealed class PauseSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("users/subscriptions/{userSubscriptionId:guid}/pause", async (
            Guid userSubscriptionId,
            ICommandHandler<PauseSubscriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new PauseSubscriptionCommand(userSubscriptionId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Users).RequireAuthorization();
    }
}
