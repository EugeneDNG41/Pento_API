using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.DietaryTags.Get;
public sealed record GetDietaryTagsQuery() : IQuery<IReadOnlyList<DietaryTagResponse>>;

public sealed record DietaryTagResponse(
    Guid Id,
    string Name,
    string? Description
);
