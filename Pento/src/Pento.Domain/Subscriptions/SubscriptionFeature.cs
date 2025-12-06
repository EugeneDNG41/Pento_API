using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.Subscriptions;

public sealed class SubscriptionFeature : Entity
{
    private SubscriptionFeature() { }
    public SubscriptionFeature(Guid id, Guid subscriptionId, string featureCode, int? quota = null, TimeUnit? resetPeriod = null) : base(id)
    {
        SubscriptionId = subscriptionId;
        FeatureCode = featureCode;
        Quota = quota;
        ResetPeriod = resetPeriod;
    }
    public Guid SubscriptionId { get; private set; }
    public string FeatureCode { get; private set; }
    public int? Quota { get; private set; }
    public TimeUnit? ResetPeriod { get; private set; }
    public static SubscriptionFeature Create(Guid subscriptionId, string featureId, int? quota = null, TimeUnit? resetPeriod = null)
    {
        var subscriptionFeature = new SubscriptionFeature(Guid.NewGuid(), subscriptionId, featureId, quota, resetPeriod);
        subscriptionFeature.Raise(new SubscriptionFeatureAddedOrUpdatedDomainEvent(subscriptionFeature.Id));
        return subscriptionFeature;
    }
    public void UpdateDetails(string? featureCode, int? quota, TimeUnit? resetPeriod)
    {
        if (!string.IsNullOrEmpty(featureCode) && FeatureCode != featureCode)
        {
            FeatureCode = featureCode;
        }
        if (Quota != quota)
        {
            Quota = quota;
        }
        if (ResetPeriod != resetPeriod)
        {
            ResetPeriod = resetPeriod;
        }
        Raise(new SubscriptionFeatureAddedOrUpdatedDomainEvent(Id));
    }
}
public sealed class SubscriptionFeatureAddedOrUpdatedDomainEvent(Guid subscriptionFeatureId) : DomainEvent
{
    public Guid SubscriptionFeatureId { get; } = subscriptionFeatureId;
}
