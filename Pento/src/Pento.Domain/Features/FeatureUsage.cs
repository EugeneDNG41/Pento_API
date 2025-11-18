using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.Features;

public sealed class FeatureUsage : Entity
{
    public Guid UserId { get; private set; }
    public Guid FeatureId { get; private set; }
    public int UsageCount { get; private set; }
    public Limit? Limit { get; private set; }
    public void IncrementUsage(int amount = 1)
    {
        UsageCount += amount;
    }
    public void ResetUsage()
    {
        UsageCount = 0;
    }
}
