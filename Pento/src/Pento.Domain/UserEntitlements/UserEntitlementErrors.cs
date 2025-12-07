using Pento.Domain.Abstractions;

namespace Pento.Domain.UserEntitlements;

public static class UserEntitlementErrors
{
    public static readonly Error NotFound =
        Error.NotFound("UserEntitlement.NotFound", "User entitlement not found.");
    public static readonly Error QuotaExceeded =
        Error.Forbidden("UserEntitlement.QuotaExceeded", "You have used up your alloted quota for this feature.");
    public static readonly Error SubscriptionNeeded =
        Error.Problem("UserEntitlement.SubscriptionNeeded", "A valid subscription is needed for this feature.");
    public static readonly Error InvalidReset =
        Error.Problem("UserEntitlement.InvalidReset", "Invalid reset period for entitlement.");
}
