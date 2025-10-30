using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Users;

public static class UserErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Users.NotFound", $"User not found");
    public static Error IdentityNotFound(string identityId) =>
        Error.NotFound("Users.IdentityNotFound", $"The user with the IDP identifier {identityId} not found");

    public static readonly Error InvalidCredentials = Error.Problem(
        "User.InvalidCredentials",
        "The provided credentials were invalid");
    public static readonly Error InvalidToken = Error.Problem(
        "User.InvalidToken",
        "The provided token is invalid");
    public static readonly Error VerificationEmailFailed = Error.Failure(
        "User.VerificationEmailFailed",
        "Failed to send verification email");
    public static readonly Error UserAlreadyInHousehold = Error.Conflict(
        "User.UserAlreadyInHousehold",
        "The user is already a member of a household");
    public static readonly Error UserNotInHousehold = Error.Problem(
        "User.UserNotInHousehold",
        "The user is not a member of the household");
}
