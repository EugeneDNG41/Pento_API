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
                request.FeatureCode,
                request.Quota,
                request.ResetPeriod), cancellationToken);
            return result
            .Match(id => Results.CreatedAtRoute(RouteNames.GetSubscriptionById, new { subscriptionId = id }, id), CustomResults.Problem);
        }).WithTags(Tags.Subscriptions);
    }
    internal sealed class Request
    {
        public string FeatureCode { get; init; }
        public int? Quota { get; init; }
        public TimeUnit? ResetPeriod { get; init; }
    }
}
