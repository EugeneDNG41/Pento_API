using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.PauseUserSubscription;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Patch;

internal sealed class PauseUserSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("admin/users/subscriptions/{userSubscriptionId:guid}/pause", async (
            Guid userSubscriptionId,
            ICommandHandler<PauseUserSubscriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new PauseUserSubscriptionCommand(userSubscriptionId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageSubscriptions);
    }
}
