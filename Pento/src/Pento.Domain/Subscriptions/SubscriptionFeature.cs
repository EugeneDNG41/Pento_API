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
    public SubscriptionFeature(Guid id, Guid subscriptionId, string featureName, Limit? entitlement) : base(id)
    {
        SubscriptionId = subscriptionId;
        FeatureName = featureName;
        Entitlement = entitlement;
    }
    public Guid SubscriptionId { get; private set; }
    public string FeatureName { get; private set; }
    public Limit? Entitlement { get; private set; }
    public static SubscriptionFeature Create(Guid subscriptionId, string featureName, Limit? entitlement)
        => new(Guid.CreateVersion7(), subscriptionId, featureName, entitlement);
}
