using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.Update;
using Pento.Domain.Abstractions;

namespace Pento.API.Endpoints.Admin.Patch;

internal sealed class UpdateSubscription : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("admin/subscriptions/{subscriptionId:guid}", async (
            Guid subscriptionId,
            Request request,
            ICommandHandler<UpdateSubscriptionCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateSubscriptionCommand(
                subscriptionId,
                request.Name,
                request.Description,
                request.IsActive), cancellationToken);
            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageSubscriptions);
    }
    internal sealed class Request
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public bool? IsActive { get; init; }
    }
}
