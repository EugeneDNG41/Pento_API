using Microsoft.AspNetCore.Mvc;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.UserSubscriptions.CancelUserSubscription;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Users.Patch;

internal sealed class CancelUserSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("admin/users/subscriptions/{userSubscriptionId:guid}/cancel", async (
            Guid userSubscriptionId,
            Request request,
            ICommandHandler<CancelUserSubscriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new CancelUserSubscriptionCommand(userSubscriptionId, request.Reason), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageSubscriptions);
    }
    internal sealed class Request
    {
        public string? Reason { get; set; }
    }
}
