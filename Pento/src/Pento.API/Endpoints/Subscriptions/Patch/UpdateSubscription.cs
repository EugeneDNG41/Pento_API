using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Subscriptions.Patch;

internal sealed class UpdateSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("subscriptions/{subscriptionId:guid}", async (
            Guid subscriptionId,
            Request request,
            ICommandHandler<UpdateSubscriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateSubscriptionCommand(
                subscriptionId,
                request.Name,
                request.Description), cancellationToken);
            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        }).WithTags(Tags.Subscriptions);
    }
    internal sealed class Request
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
    }
}
