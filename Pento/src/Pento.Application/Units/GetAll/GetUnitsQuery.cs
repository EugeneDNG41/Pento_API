using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Units.Get;

namespace Pento.Application.Units.GetAll;

public sealed record GetUnitsQuery() : IQuery<IReadOnlyList<UnitResponse>>;
