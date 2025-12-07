using Pento.Domain.Abstractions;

namespace Pento.Domain.Storages;

public static class StorageErrors
{
    public static Error NotFound => Error.NotFound(
        "Storage.NotFound",
        "Storage not found.");
    public static Error ForbiddenAccess => Error.Forbidden(
        "Storage.ForbiddenAccess",
        "You do not have permission to access this storage.");
    public static Error NotEmpty => Error.Conflict(
        "Storage.HasActiveUsers",
        "Storage is not empty."); //business rule: cannot delete a storage that still has items within compartments
    public static Error DuplicateName => Error.Conflict(
        "Storage.DuplicateName",
        "A storage with the same name already exists."); //business rule: storage names must be unique within a household
    public static Error AtLeastOne => Error.Conflict(
        "Storage.AtLeastOne",
        "There must be at least one storage."); //business rule: a household must have at least one storage
    public static Error StorageLimitReached => Error.Conflict(
        "Storage.LimitReached",
        "The maximum number of storages has been reached."); //business rule: limit the number of storages per household
}
