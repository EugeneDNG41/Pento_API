using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.UpdateFeature;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.API.Endpoints.Subscriptions.Patch;

internal sealed class UpdateSubscriptionFeature : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("subscriptions/features/{subscriptionFeatureId:guid}", async (
            Guid subscriptionFeatureId,
            Request request,
            ICommandHandler<UpdateSubscriptionFeatureCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateSubscriptionFeatureCommand(
                subscriptionFeatureId,
                request.FeatureCode,
                request.EntitlementQuota,
                request.EntitlementResetPer), cancellationToken);
            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        }).WithTags(Tags.Subscriptions);
    }
    internal sealed class Request
    {
        public string FeatureCode { get; init; }
        public int? EntitlementQuota { get; init; }
        public TimeUnit? EntitlementResetPer { get; init; }
    }

}
