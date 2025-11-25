using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.UserEntitlements;

public sealed class UserEntitlement : Entity
{
    private UserEntitlement() { }
    public UserEntitlement(Guid id, Guid userId, Feature feature, Limit? limit) : base(id)
    {
        UserId = userId;
        Feature = feature;
        Limit = limit;
        UsageCount = 0;
    }
    public Guid UserId { get; private set; }
    public Feature Feature { get; private set; }
    public int UsageCount { get; private set; }
    public Limit? Limit { get; private set; }
    public static UserEntitlement Create(Guid userId, Feature feature, Limit? limit)
        => new(Guid.CreateVersion7(), userId, feature, limit);
    public void IncrementUsage(int amount = 1)
    {
        UsageCount += amount;
    }
    public void ResetUsage()
    {
        UsageCount = 0;
    }
}
public static class UserEntitlementErrors
{
    public static readonly Error NotFound = 
        Error.NotFound("UserEntitlement.NotFound", "User entitlement not found.");
    public static readonly Error LimitExceeded = 
        Error.Forbidden("UserEntitlement.LimitExceeded", "User entitlement limit exceeded.");
}
