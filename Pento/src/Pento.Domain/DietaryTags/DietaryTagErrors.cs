using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.DietaryTags;
public static class DietaryTagErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "DietaryTag.NotFound",
        "The specified dietary tag was not found."
    );

    public static readonly Error DuplicateName = Error.Conflict(
        "DietaryTag.DuplicateName",
        "A dietary tag with the same name already exists."
    );
}
