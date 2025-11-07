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
    public PointCategory Category { get; private set; }
    public int CapAmount { get; private set; }
    public Interval ResetInterval { get; private set; }
    private PointCap() { }
    public PointCap(PointCategory category, int capAmount, Interval resetInterval)
    {
        Id = Guid.CreateVersion7();
        Category = category;
        CapAmount = capAmount;
        ResetInterval = resetInterval;
    }
    public static PointCap Create(PointCategory category, int capAmount, Interval resetInterval)
        => new(category, capAmount, resetInterval);
}
