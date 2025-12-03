using Microsoft.AspNetCore.Mvc;
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.UserSubscriptions.AdjustUserSubscription;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Patch;

internal sealed class AdjustUserSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("users/subscriptions/{userSubscriptionId:guid}/adjust", async (
            Guid userSubscriptionId,
            [FromBody] int durationInDays,
            ICommandHandler<AdjustUserSubscriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new AdjustUserSubscriptionCommand(userSubscriptionId, durationInDays), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageSubscriptions);
    }
}
