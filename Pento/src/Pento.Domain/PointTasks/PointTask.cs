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
    public string Description { get; private set; }
    public string Instructions { get; private set; }
    public ActivityType ActivityType { get; private set; }
    public int Weight { get; private set; }
    public Limit Limit { get; private set; }
}
