namespace Pento.Application.Subscriptions.GetById;

public sealed record SubscriptionDetailResponse(
    Guid SubscriptionId,
    string Name,
    string Description,
    IReadOnlyList<SubscriptionPlanResponse> Plans,
    IReadOnlyList<SubscriptionFeatureResponse> Features
);
public sealed record SubscriptionResponse
{
    public Guid SubscriptionId { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
}
public sealed record SubscriptionPlanResponse
{
    public Guid SubscriptionPlanId { get; init; }
    public string Price { get; init; }
    public string Duration { get; init; }
}
public sealed record SubscriptionFeatureResponse
{
    public Guid SubscriptionFeatureId { get; init; }
    public string FeatureName { get; init; }
    public string Entitlement { get; init; }
}
