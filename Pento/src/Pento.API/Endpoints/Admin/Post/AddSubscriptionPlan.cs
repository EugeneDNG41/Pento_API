using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.AddPlan;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.API.Endpoints.Admin.Post;

internal sealed class AddSubscriptionPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("admin/subscriptions/{subscriptionId:guid}/plans", async (
            Guid subscriptionId,
            Request request,
            ICommandHandler<AddSubscriptionPlanCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(new AddSubscriptionPlanCommand(
                subscriptionId,
                request.Amount,
                request.Currency,
                request.DurationInDays), cancellationToken);
            return result
            .Match(id => Results.CreatedAtRoute(RouteNames.GetSubscriptionById, new { subscriptionId = id }, id), CustomResults.Problem);
        }).WithTags(Tags.Admin).RequireAuthorization(Permissions.ManageSubscriptions);
    }
    internal sealed class Request
    {
        public long Amount { get; init; }
        public Currency Currency { get; init; }
        public int? DurationInDays { get; init; }
    }
}
