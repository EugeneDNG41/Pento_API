using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Households;

public static class HouseholdErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Household.IdentityNotFound",
        "Household not found.");

    public static readonly Error InviteCodeNotFound = Error.NotFound(
        "Household.InviteCodeNotFound",
        "The household with this invite code was not found.");
    public static readonly Error InviteCodeExpired = Error.Problem(
        "Household.InviteCodeExpired",
        "The invite code has expired.");
    public static readonly Error AlreadyInThisHousehold = Error.Conflict(
        "User.UserAlreadyInHousehold",
        "You are already in this household");

    public static readonly Error NotInAnyHouseHold = Error.Forbidden(
        "User.NotInThisHouseHold",
        "You are not in any household"); //when a user in a household try to affect change in another household's data

    public static readonly Error CannotRemoveSelf = Error.Problem(
        "User.CannotRemoveSelf",
        "You cannot remove yourself from the household");
    public static readonly Error CannotAssignRolesSelf = Error.Problem(
        "User.CannotAssignRolesSelf",
        "You cannot set your own roles");
    public static readonly Error HasActiveUsers = Error.Conflict(
        "Household.HasActiveUsers",
        "Cannot delete household with active users.");
}
