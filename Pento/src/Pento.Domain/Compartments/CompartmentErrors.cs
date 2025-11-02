using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        "Compartment.NotEmpty",
        "Compartment is not empty.");
}
