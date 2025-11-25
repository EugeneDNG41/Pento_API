using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Subscriptions;

public sealed class Subscription : Entity
{
    private Subscription() { }
    public Subscription(Guid id, string name, string description) : base(id)
    {
        Name = name;
        Description = description;
    }
    public string Name { get; private set; }
    public string Description { get; private set; }

    public static Subscription Create(string name, string description)
        => new(Guid.CreateVersion7(), name, description);
    public void UpdateDetails(string? name, string? description)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description;
        }
    }
}
public static class SubscriptionErrors
{
    public static readonly Error SubscriptionNotFound = 
        Error.NotFound("Subscription.NotFound", "Subscription not found.");
    public static readonly Error SubscriptionPlanNotFound = 
        Error.NotFound("SubscriptionPlan.NotFound", "Subscription plan not found.");
    public static readonly Error SubscriptionFeatureNotFound = 
        Error.NotFound("SubscriptionFeature.NotFound", "Subscription feature not found.");
}

