using Pento.Domain.Abstractions;

namespace Pento.Domain.UserEntitlements;

public static class UserEntitlementErrors
{
    public static readonly Error NotFound = 
        Error.NotFound("UserEntitlement.NotFound", "User entitlement not found.");
    public static readonly Error LimitExceeded = 
        Error.Forbidden("UserEntitlement.LimitExceeded", "User entitlement limit exceeded.");
}
