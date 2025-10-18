﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Units.Get;
public sealed record GetUnitQuery(Guid UnitId) : IQuery<UnitResponse>;

