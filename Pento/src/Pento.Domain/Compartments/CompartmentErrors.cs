using Pento.Domain.Abstractions;

namespace Pento.Domain.Compartments;

public static class CompartmentErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Compartment.NotFound",
        "Compartment not found.");
    public static readonly Error ForbiddenAccess = Error.Forbidden(
        "Compartment.ForbiddenAccess",
        "You do not have permission to access this compartment.");
    public static readonly Error NotEmpty = Error.Conflict(
        "Compartment.HasActiveUsers",
        "Compartment is not empty.");
    public static readonly Error DuplicateName = Error.Conflict(
        "Compartment.DuplicateName",
        "A compartment with the same name already exists.");
    public static readonly Error AtLeastOne = Error.Conflict(
        "Compartment.AtLeastOne",
        "There must be at least one compartment.");
}
