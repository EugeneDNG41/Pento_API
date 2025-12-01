using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.DeletePlan;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Subscriptions.Delete;

internal sealed class DeleteSubscriptionPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("admin/subscriptions/plans/{subscriptionPlanId:guid}", async (
            Guid subscriptionPlanId,
            ICommandHandler<DeleteSubscriptionPlanCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new DeleteSubscriptionPlanCommand(subscriptionPlanId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageSubscriptions);
    }
}
