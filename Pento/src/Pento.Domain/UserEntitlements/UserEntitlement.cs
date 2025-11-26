using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.UserEntitlements;

public sealed class UserEntitlement : Entity
{
    private UserEntitlement() { }
    public UserEntitlement(Guid id, Guid userId, string featureName, Limit? entitlement) : base(id)
    {
        UserId = userId;
        FeatureName = featureName;
        Entitlement = entitlement;
        UsageCount = 0;
    }
    public Guid UserId { get; private set; }
    public string FeatureName { get; private set; }
    public int UsageCount { get; private set; }
    public Limit? Entitlement { get; private set; }
    public static UserEntitlement Create(Guid userId, string featureName, Limit? entitlement)
        => new(Guid.CreateVersion7(), userId, featureName, entitlement);
    public void IncrementUsage(int amount = 1)
    {
        UsageCount += amount;
    }
    public void ResetUsage()
    {
        UsageCount = 0;
    }
}
