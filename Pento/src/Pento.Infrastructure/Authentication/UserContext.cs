using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Authentication;

namespace Pento.Infrastructure.Authentication;

internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId =>
        httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
        throw new ApplicationException("User context is unavailable");
    public Guid? HouseholdId =>
        httpContextAccessor
            .HttpContext?
            .User
            .GetHouseholdId();

    public string IdentityId =>
        httpContextAccessor
            .HttpContext?
            .User
            .GetIdentityId() ??
        throw new ApplicationException("User context is unavailable");
    public bool IsDeleted =>
        httpContextAccessor
            .HttpContext?
            .User
            .IsDeleted() ??
        throw new ApplicationException("User context is unavailable");
}
