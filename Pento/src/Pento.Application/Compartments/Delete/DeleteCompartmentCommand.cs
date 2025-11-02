using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Compartments.Delete;

public sealed record DeleteCompartmentCommand(Guid CompartmentId) : ICommand;
