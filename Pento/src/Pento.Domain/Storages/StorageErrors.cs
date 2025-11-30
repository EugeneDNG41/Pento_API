using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        "Storage.NotEmpty",
        "Storage is not empty.");
    public static Error DuplicateName => Error.Conflict(
        "Storage.DuplicateName",
        "A storage with the same name already exists.");
    public static Error AtLeastOne => Error.Conflict(
        "Storage.AtLeastOne",
        "There must be at least one storage.");
}
