using Pento.Domain.Abstractions;
using Pento.Domain.Milestones;
using Pento.Domain.UserActivities;

namespace Pento.Application.Abstractions.Services;

public interface IMilestoneService
{
    Task<Result> CheckMilestoneAfterActivityAsync(UserActivity userActivity, CancellationToken cancellationToken);
    Task<Result> CheckMilestoneWithSaveChangesAsync(UserActivity userActivity, CancellationToken cancellationToken);
    Task<Result> CheckUserMilestoneAsync(Guid userId, Milestone milestone, CancellationToken cancellationToken);
}
