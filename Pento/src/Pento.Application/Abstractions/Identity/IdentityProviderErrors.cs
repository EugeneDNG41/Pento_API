using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.Identity;

public static class IdentityProviderErrors
{
    public static readonly Error EmailIsNotUnique = Error.Conflict(
        "Identity.EmailIsNotUnique",
        "The specified email is not unique.");
    public static readonly Error AuthenticationFailed = Error.Failure(
        "Identity.AuthenticationFailed",
        "Failed to acquire access token");
}
