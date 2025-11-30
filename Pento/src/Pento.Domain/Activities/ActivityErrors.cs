using Pento.Domain.Abstractions;

namespace Pento.Domain.Activities;

public static class ActivityErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Activity.NotFound",
        "Activity not found."
    );
    public static readonly Error UserActivityNotFound = Error.NotFound(
        "UserActivity.NotFound",
        "User activity not found."
    );
    public static readonly Error NameTaken = Error.Conflict(
        "Activity.NameTaken",
        "An activity with the same name already exists."
    );
}
