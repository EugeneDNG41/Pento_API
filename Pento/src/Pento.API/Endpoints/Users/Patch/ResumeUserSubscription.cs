using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.UserSubscriptions.ResumeUserSubscription;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Patch;

internal sealed class ResumeUserSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("admin/users/subscriptions/{userSubscriptionId:guid}/resume", async (
            Guid userSubscriptionId,
            ICommandHandler<ResumeUserSubscriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new ResumeUserSubscriptionCommand(userSubscriptionId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageSubscriptions);
    }
}
