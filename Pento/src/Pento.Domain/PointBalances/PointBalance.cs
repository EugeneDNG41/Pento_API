using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.PointBalances;

public sealed class PointBalance
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public int Balance { get; private set; }
    public int Version { get; private set; }
    }
