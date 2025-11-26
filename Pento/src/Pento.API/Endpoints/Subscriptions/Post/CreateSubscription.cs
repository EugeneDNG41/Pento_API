using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.Create;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Subscriptions.Post;

internal sealed class CreateSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("subscriptions", async (
            Request request,
            ICommandHandler<CreateSubscriptionCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(new CreateSubscriptionCommand(request.Name, request.Description), cancellationToken);
            return result
            .Match(id => Results.CreatedAtRoute(RouteNames.GetSubscriptionById, new { subscriptionId = id }, id), CustomResults.Problem);
        }).WithTags(Tags.Subscriptions);
    }
    internal sealed class Request
    {
        public string Name { get; init; }
        public string Description { get; init; }
    }
}
