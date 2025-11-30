using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.ThirdPartyServices.Identity;

public static class IdentityProviderErrors
{
    public static readonly Error EmailTaken = Error.Conflict(
        "Identity.EmailTaken",
        "This email is already in use");
    public static readonly Error AuthenticationFailed = Error.Failure(
        "Identity.AuthenticationFailed",
        "Failed to acquire access token");
}
