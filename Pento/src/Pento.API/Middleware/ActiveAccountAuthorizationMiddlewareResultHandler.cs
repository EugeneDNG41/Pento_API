using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Pento.API.Extensions;
using Pento.Domain.Abstractions;
using Pento.Domain.Users;
using Pento.Infrastructure.Authentication;

namespace Pento.API.Middleware;

internal sealed class ActiveAccountAuthorizationMiddlewareResultHandler
    : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (context.User.Identity?.IsAuthenticated == true &&
            context.User.IsDeleted())
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                title = "User.AccountDeleted",
                status = 403,
                detail = "Your account has been deleted.",
            });

            return;
        }
        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
