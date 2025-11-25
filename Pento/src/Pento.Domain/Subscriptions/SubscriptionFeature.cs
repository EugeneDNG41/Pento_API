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
    public SubscriptionFeature(Guid id, Guid subscriptionId, Feature feature, Limit? limit) : base(id)
    {
        SubscriptionId = subscriptionId;
        Feature = feature;
        Limit = limit;
    }
    public Guid SubscriptionId { get; private set; }
    public Feature Feature { get; private set; }
    public Limit? Limit { get; private set; }
    public static SubscriptionFeature Create(Guid subscriptionId, Feature feature, Limit? limit)
        => new(Guid.CreateVersion7(), subscriptionId, feature, limit);
}
