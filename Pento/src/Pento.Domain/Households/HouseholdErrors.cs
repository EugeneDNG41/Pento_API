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
}
