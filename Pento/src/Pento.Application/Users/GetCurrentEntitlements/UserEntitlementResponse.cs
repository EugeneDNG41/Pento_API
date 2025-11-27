namespace Pento.Application.Users.GetCurrentEntitlements;

public sealed record UserEntitlementResponse
{
    public string FeatureName { get; init; }
    public string FeatureDescription { get; init; }
    public string Entitlement { get; init; }
    public string FromSubcription { get; init; }
}

