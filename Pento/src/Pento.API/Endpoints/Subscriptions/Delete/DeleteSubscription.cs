using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.Delete;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Subscriptions.Delete;

internal sealed class DeleteSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("subscriptions/{subscriptionId:guid}", async (
            Guid subscriptionId,
            ICommandHandler<DeleteSubscriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new DeleteSubscriptionCommand(subscriptionId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Subscriptions);
    }
}
