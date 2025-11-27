namespace Pento.Application.Features.GetAll;

public sealed record FeatureResponse
{
    public string FeatureCode { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string DefaultEntitlement { get; init; }
}
