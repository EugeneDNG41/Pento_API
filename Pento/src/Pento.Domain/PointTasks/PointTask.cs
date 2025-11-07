using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.PointTasks;

public sealed class PointTask
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public PointCategory Category { get; private set; }
    public int Weight { get; private set; }
    public bool IsRepeatable { get; private set; }
}
