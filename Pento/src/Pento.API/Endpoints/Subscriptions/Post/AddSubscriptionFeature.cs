using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.AddFeature;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.API.Endpoints.Subscriptions.Post;

internal sealed class AddSubscriptionFeature : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("subscriptions/{subscriptionId:guid}/features", async (
            Guid subscriptionId,
            Request request,
            ICommandHandler<AddSubscriptionFeatureCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(new AddSubscriptionFeatureCommand(
                subscriptionId,
                request.FeatureName,
                request.EntitlementQuota,
                request.EntitlementResetPer), cancellationToken);
            return result
            .Match(id => Results.CreatedAtRoute(RouteNames.GetSubscriptionById, new { subscriptionId = id }, id), CustomResults.Problem);
        }).WithTags(Tags.Subscriptions);
    }
    internal sealed class Request
    {
        public string FeatureName { get; init; }
        public int? EntitlementQuota { get; init; }
        public TimeUnit? EntitlementResetPer { get; init; }
    }
}
