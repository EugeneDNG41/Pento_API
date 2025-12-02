using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Milestones.GetById;

public sealed record GetAdminMilestoneByIdQuery(Guid Id) : IQuery<AdminMilestoneDetailResponse>;

