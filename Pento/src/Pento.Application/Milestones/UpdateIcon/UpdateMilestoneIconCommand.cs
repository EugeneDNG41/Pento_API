using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Milestones.UpdateIcon;

public sealed record UpdateMilestoneIconCommand(Guid MilestoneId, IFormFile IconFile) : ICommand<Uri>;

