using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.External.Identity;

public static class IdentityProviderErrors
{
    public static readonly Error EmailTaken = Error.Conflict(
        "Identity.EmailTaken",
        "This email is already in use");
    public static readonly Error AuthenticationFailed = Error.Failure(
        "Identity.AuthenticationFailed",
        "Failed to acquire access token");
    public static readonly Error InvalidCredentials = Error.Problem(
        "Identity.InvalidCredentials",
        "The provided credentials were invalid");
    public static readonly Error InvalidToken = Error.Problem(
        "Identity.InvalidToken",
        "The provided token is invalid");
    public static readonly Error VerificationEmailFailed = Error.Failure(
        "Identity.VerificationEmailFailed",
        "Failed to send verification email");
}
