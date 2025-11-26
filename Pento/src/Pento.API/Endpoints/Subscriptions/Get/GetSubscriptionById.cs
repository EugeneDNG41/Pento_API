
using Pento.API.Extensions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Subscriptions.Create;
using Pento.Application.Subscriptions.GetById;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.API.Endpoints.Subscriptions.Get;

internal sealed class GetSubscriptionById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("subscriptions/{subscriptionId:guid}", async (
            Guid subscriptionId,
            IQueryHandler<GetSubscriptionByIdQuery, SubscriptionDetailResponse> handler,
            CancellationToken cancellationToken) =>
        {
            Result<SubscriptionDetailResponse> result = await handler.Handle(new GetSubscriptionByIdQuery(subscriptionId), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Subscriptions).WithName(RouteNames.GetSubscriptionById);
    }
}

internal sealed class GetFeatures : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("features", async (
            IQueryHandler<GetFeaturesQuery, IReadOnlyList<FeatureResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<FeatureResponse>> result = await handler.Handle(new GetFeaturesQuery(), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Features);
    }
}

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

internal sealed class UpdateSubscriptionPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("subscriptions/plans/{subscriptionPlanId:guid}", async (
            Guid subscriptionPlanId,
            Request request,
            ICommandHandler<UpdateSubscriptionPlanCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new UpdateSubscriptionPlanCommand(
                subscriptionPlanId,
                request.PriceAmount,
                request.PriceCurrency,
                request.DurationValue,
                request.DurationUnit), cancellationToken);
            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        }).WithTags(Tags.Subscriptions);
    }
    internal sealed class Request
    {
        public long? PriceAmount { get; init; }
        public string? PriceCurrency { get; init; }
        public int? DurationValue { get; init; }
        public TimeUnit? DurationUnit { get; init; }
    }
}
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
                request.FeatureName,
                request.EntitlementQuota,
                request.EntitlementResetPer), cancellationToken);
            return result.Match(() => Results.NoContent(), CustomResults.Problem);
        }).WithTags(Tags.Subscriptions);
    }
    internal sealed class Request
    {
        public string FeatureName { get; init; }
        public int? EntitlementQuota { get; init; }
        public TimeUnit? EntitlementResetPer { get; init; }
    }

}
