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
    private PointCap() { }
    public PointCap(ActivityType category)
    {
        Id = Guid.CreateVersion7();
        Category = category;
    }
    public static PointCap Create(ActivityType category)
        => new(category);
}
