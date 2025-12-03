using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Milestones.UpdateIcon;

public sealed record UpdateMilestoneIconCommand(Guid MilestoneId, IFormFile IconFile) : ICommand<Uri>;

