using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.UpdatePlan;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.API.Endpoints.Admin.Patch;

internal sealed class UpdateSubscriptionPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("admin/subscriptions/plans/{subscriptionPlanId:guid}", async (
            Guid subscriptionPlanId,
            Request request,
            ICommandHandler<UpdateSubscriptionPlanCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateSubscriptionPlanCommand(
                subscriptionPlanId,
                request.Amount,
                request.Currency,
                request.DurationInDays), cancellationToken);
            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageSubscriptions);
    }
    internal sealed class Request
    {
        public long? Amount { get; init; }
        public Currency? Currency { get; init; }
        public int? DurationInDays { get; init; }
    }
}
