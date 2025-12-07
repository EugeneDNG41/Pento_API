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
        "Compartment is not empty."); //business rule: cannot delete a compartment that still has items
    public static readonly Error DuplicateName = Error.Conflict(
        "Compartment.DuplicateName",
        "A compartment with the same name already exists."); //business rule: compartment names must be unique within a storage
    public static readonly Error AtLeastOne = Error.Conflict( 
        "Compartment.AtLeastOne",
        "There must be at least one compartment."); //business rule: a storage must have at least one compartment
    public static readonly Error CompartmentLimitReached = Error.Conflict(
        "Compartment.LimitReached",
        "The maximum number of compartments has been reached."); //business rule: limit the number of compartments per storage
}
