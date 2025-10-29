using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) =>
        Error.NotFound("Users.NotFound", $"The user with the identifier {userId} not found");

    public static Error NotFound(string identityId) =>
        Error.NotFound("Users.NotFound", $"The user with the IDP identifier {identityId} not found");

    public static readonly Error InvalidCredentials = Error.Problem(
        "User.InvalidCredentials",
        "The provided credentials were invalid");
    public static readonly Error InvalidToken = Error.Problem(
        "User.InvalidToken",
        "The provided token is invalid");
}
