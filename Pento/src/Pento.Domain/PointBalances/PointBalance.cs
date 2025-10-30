﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.PointBalances;

public sealed class PointBalance : Entity
{
    public ClientScope Scope { get; private set; }
    public Guid ScopeId { get; private set; }
    public Guid PointCategoryId { get; private set; }
    public int Balance { get; private set; }
}
