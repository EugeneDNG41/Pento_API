using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.UserMilestones.GetById;

public sealed record GetMilestoneByIdQuery(Guid MilestoneId) : IQuery<UserMilestoneDetailResponse>;
