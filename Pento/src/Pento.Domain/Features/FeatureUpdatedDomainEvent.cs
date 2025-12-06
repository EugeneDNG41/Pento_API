using Pento.Domain.Abstractions;

namespace Pento.Domain.Features;

public sealed class FeatureUpdatedDomainEvent(string featureCode) : DomainEvent
{
    public string FeatureCode { get; } = featureCode;
}
