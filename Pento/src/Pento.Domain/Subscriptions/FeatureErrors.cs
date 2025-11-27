using Pento.Domain.Abstractions;

namespace Pento.Domain.Subscriptions;

public static class FeatureErrors
{
    public static readonly Error NotFound = Error.NotFound("Feature.NotFound", "Feature not found.");
    public static readonly Error NameTaken = Error.Conflict("Feature.NameTaken", "Feature name already taken.");

}
