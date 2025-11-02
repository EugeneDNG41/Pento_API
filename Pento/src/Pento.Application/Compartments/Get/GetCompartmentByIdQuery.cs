using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Storages.GetById;

namespace Pento.Application.Compartments.Get;

public sealed record GetCompartmentByIdQuery(Guid CompartmentId) : IQuery<CompartmentResponse>;
