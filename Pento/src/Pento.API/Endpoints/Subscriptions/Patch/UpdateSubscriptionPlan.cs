using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.UpdatePlan;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.API.Endpoints.Subscriptions.Patch;

internal sealed class UpdateSubscriptionPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("subscriptions/plans/{subscriptionPlanId:guid}", async (
            Guid subscriptionPlanId,
            Request request,
            ICommandHandler<UpdateSubscriptionPlanCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateSubscriptionPlanCommand(
                subscriptionPlanId,
                request.PriceAmount,
                request.PriceCurrency,
                request.DurationValue,
                request.DurationUnit), cancellationToken);
            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        }).WithTags(Tags.Subscriptions);
    }
    internal sealed class Request
    {
        public long? PriceAmount { get; init; }
        public string? PriceCurrency { get; init; }
        public int? DurationValue { get; init; }
        public TimeUnit? DurationUnit { get; init; }
    }
}
