using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Pento.Infrastructure.Authorization;

public class DenyAnonymousAuthorizationRequirement : AuthorizationHandler<DenyAnonymousAuthorizationRequirement>, IAuthorizationRequirement
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DenyAnonymousAuthorizationRequirement requirement)
    {
        ClaimsPrincipal user = context.User;
        bool userIsAnonymous =
            user?.Identity == null ||
            !user.Identities.Any(i => i.IsAuthenticated);
        if (!userIsAnonymous)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(DenyAnonymousAuthorizationRequirement)}: Requires an authenticated user.";
    }
}

