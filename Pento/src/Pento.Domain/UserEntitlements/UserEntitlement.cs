using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.UserEntitlements;

public sealed class UserEntitlement : Entity
{
    private UserEntitlement() { }
    public UserEntitlement(Guid id,  Guid userId, Guid? userSubscription, string featureCode, int? quota = null, TimeUnit? resetPeriod = null)
        : base(id)
    {
        UserId = userId;
        UserSubscriptionId = userSubscription;
        FeatureCode = featureCode;
        UsageCount = 0;
        Quota = quota;
        ResetPeriod = resetPeriod;
    }
    public Guid UserId { get; private set; }
    public Guid? UserSubscriptionId { get; private set; }
    public string FeatureCode { get; private set; }
    public int UsageCount { get; private set; }
    public int? Quota { get; private set; }
    public TimeUnit? ResetPeriod { get; private set; }
    public static UserEntitlement Create(Guid userId, Guid? userSubscription, string featureCode, int? quota = null, TimeUnit? resetPeriod = null)
        => new(Guid.CreateVersion7(), userId, userSubscription, featureCode, quota, resetPeriod);
    public void IncrementUsage(int amount = 1)
    {
        UsageCount += amount;
    }
    public void ResetUsage()
    {
        UsageCount = 0;
    }
    public void UpdateEntitlement(int? quota = null, TimeUnit? resetPeriod = null)
    {
        Quota = quota;
        ResetPeriod = resetPeriod;
    }
}
