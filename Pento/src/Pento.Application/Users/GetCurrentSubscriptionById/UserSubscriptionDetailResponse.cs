using Pento.Application.Users.GetCurrentSubscriptions;

namespace Pento.Application.Users.GetCurrentSubscriptionById;

public sealed record UserSubscriptionDetailResponse(
    UserSubscriptionResponse Subscription, 
    IReadOnlyList<UserEntitlementBySubscription> Entitlements);
public sealed record UserEntitlementBySubscription
{
    public string FeatureName { get; init; }
    public string FeatureDescription { get; init; }
    public string Entitlement { get; init; }
}
