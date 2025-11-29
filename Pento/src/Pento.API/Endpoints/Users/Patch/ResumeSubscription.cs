using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Users.ResumeSubscription;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Patch;

internal sealed class ResumeSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("users/subscriptions/{userSubscriptionId:guid}/resume", async (
            Guid userSubscriptionId,
            ICommandHandler<ResumeSubscriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new ResumeSubscriptionCommand(userSubscriptionId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Users).RequireAuthorization();
    }
}
