using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.DietaryTags.Update;
public sealed record UpdateDietaryTagCommand(
    Guid Id,
    string Name,
    string? Description
) : ICommand<Guid>;
