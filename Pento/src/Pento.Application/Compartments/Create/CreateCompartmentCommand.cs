using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Units;

namespace Pento.Application.Compartments.Create;

public sealed record CreateCompartmentCommand(Guid StorageId, string Name, string? Notes) : ICommand<Guid>;
