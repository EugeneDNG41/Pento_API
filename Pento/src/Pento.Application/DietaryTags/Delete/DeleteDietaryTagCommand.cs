using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.DietaryTags.Delete;
public sealed record DeleteDietaryTagCommand(Guid Id) : ICommand<Guid>;
