using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.DietaryTags.Get;
public sealed record GetDietaryTagByIdQuery(Guid Id) : IQuery<DietaryTagResponse>;
