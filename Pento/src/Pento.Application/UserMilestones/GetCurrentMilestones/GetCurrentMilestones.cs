using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.UserMilestones.GetCurrentMilestones;

public sealed record GetCurrentMilestones(string? SearchTerm);

public sealed record CurrentUserMilestonesResponse(
    Guid MilestoneId,
    string Name,
    DateTime? AchievedOn);
