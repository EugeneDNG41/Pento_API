using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.UserSubscriptions.CancelSubscription;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Patch;

internal sealed class CancelSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("users/subscriptions/{userSubscriptionId:guid}/cancel", async (
            Guid userSubscriptionId,
            Request request,
            ICommandHandler<CancelSubscriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new CancelSubscriptionCommand(userSubscriptionId, request.Reason), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Users).RequireAuthorization();
    }
    internal sealed class Request
    {
        public string? Reason { get; set; }
    }
}
