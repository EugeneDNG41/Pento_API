using System.Security.Claims;
using Pento.Application.Abstractions.Exceptions;

namespace Pento.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirst(CustomClaims.Sub)?.Value;

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            throw new PentoException("User identifier is unavailable");
    }
    public static Guid GetHouseholdId(this ClaimsPrincipal? principal)
    {
        string? householdId = principal?.FindFirst(CustomClaims.Household)?.Value;

        return Guid.TryParse(householdId, out Guid parsedHouseholdId) ?
            parsedHouseholdId :
            throw new PentoException("User household is unavailable");
    }

    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
               throw new PentoException("User identity is unavailable");
    }

    public static HashSet<string> GetPermissions(this ClaimsPrincipal? principal)
    {
        IEnumerable<Claim> permissionClaims = principal?.FindAll(CustomClaims.Permission) ??
                                              throw new PentoException("Permissions are unavailable");

        return permissionClaims.Select(c => c.Value).ToHashSet();
    }
}
