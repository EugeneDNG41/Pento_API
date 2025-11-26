using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.AddPlan;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.API.Endpoints.Subscriptions.Post;

internal sealed class AddSubscriptionPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("subscriptions/{subscriptionId:guid}/plans", async (
            Guid subscriptionId,
            Request request,
            ICommandHandler<AddSubscriptionPlanCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(new AddSubscriptionPlanCommand(
                subscriptionId,
                request.PriceAmount,
                request.PriceCurrency,
                request.DurationValue,
                request.DurationUnit), cancellationToken);
            return result
            .Match(id => Results.CreatedAtRoute(RouteNames.GetSubscriptionById, new { subscriptionId = id }, id), CustomResults.Problem);
        }).WithTags(Tags.Subscriptions);
    }
    internal sealed class Request
    {
        public long PriceAmount { get; init; }
        public string PriceCurrency { get; init; }
        public int? DurationValue { get; init; }
        public TimeUnit? DurationUnit { get; init; }
    }
}
