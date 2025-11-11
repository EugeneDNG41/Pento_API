using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Shared;

namespace Pento.Domain.PointCaps;

public sealed class PointCap
{
    public Guid Id { get; private set; }
    public ActivityType Category { get; private set; }
    public Limit Limit { get; private set;  }
    public Period ResetPeriod { get; private set; } //convert to limit by period later
    private PointCap() { }
    public PointCap(ActivityType category, Limit limit)
    {
        Id = Guid.CreateVersion7();
        Category = category;
        Limit = limit;
    }
    public static PointCap Create(ActivityType category, Limit limit)
        => new(category, limit);
}
