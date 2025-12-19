using Pento.Domain.Abstractions;

namespace Pento.Domain.Users;

public static class UserErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Users.NotFound", $"User not found");
    public static Error IdentityNotFound(string identityId) =>
        Error.NotFound("Users.IdentityNotFound", $"The user with the IDP identifier {identityId} not found");
    public static readonly Error ForbiddenAccess =
        Error.Forbidden("Users.ForbiddenAccess", "You do not have permission to access this user");
    public static readonly Error CannotDeleteSelf =
        Error.Problem("Users.CannotDeleteSelf", "You cannot delete your own user account");
    public static readonly Error AccountDeleted =
        Error.Conflict("Users.AccountDeleted", "Your account has been deleted.");

}
