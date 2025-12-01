using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.DeleteFeature;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Subscriptions.Delete;

internal sealed class DeleteSubscriptionFeature : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("admin/subscriptions/features/{subscriptionFeatureId:guid}", async (
            Guid subscriptionFeatureId,
            ICommandHandler<DeleteSubscriptionFeatureCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new DeleteSubscriptionFeatureCommand(subscriptionFeatureId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageSubscriptions);
    }
}
