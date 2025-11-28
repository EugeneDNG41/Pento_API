using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.Milestones;

public sealed class Milestone : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
}

public sealed class UserMilestone
{
    public Guid UserId { get; set; }
    public Guid MilestoneId { get; private set; }
    public DateTime AchievedOnUtc { get; private set; }
}

public sealed class MilestoneRequirement : Entity
{
    public Guid MilestoneId { get; private set; }
    public string ActivityCode { get; private set; }
    public int Quota { get; private set; }
    public TimeUnit? TimeFrame { get; private set; }

}
public sealed class Activity
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
}
public sealed class UserActivity
{
    public Guid UserId { get; private set; }
    public string ActivityCode { get; private set; }
    public DateTime PerformedOnUtc { get; private set; }
    public int Quantity { get; private set; }
}
