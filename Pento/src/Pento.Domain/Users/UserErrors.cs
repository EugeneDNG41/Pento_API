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

    public static readonly Error UserNotInYourHousehold = Error.Problem(
        "User.UserNotInYourHousehold",
        "The user is not a member of your household"); //when user in a household try to affect change in another user in another household

    public static readonly Error NotInThisHouseHold = Error.Forbidden(
        "User.NotInThisHouseHold",
        "You are not in this household"); //when a user in a household try to affect change in another household's data
    public static readonly Error NotInAnyHouseHold = Error.Forbidden(
        "User.NotInThisHouseHold",
        "You are not in any household"); //when a user in a household try to affect change in another household's data

    public static readonly Error CannotRemoveSelf = Error.Problem(
        "User.CannotRemoveSelf",
        "You cannot remove yourself from the household");
}
