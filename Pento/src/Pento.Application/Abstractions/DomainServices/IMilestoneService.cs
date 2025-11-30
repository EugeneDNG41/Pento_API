using Pento.Domain.Abstractions;
using Pento.Domain.UserActivities;

namespace Pento.Application.Abstractions.DomainServices;

public interface IMilestoneService
{
    Task<Result> CheckMilestoneAsync(UserActivity userActivity, CancellationToken cancellationToken);
    Task<Result> CheckMilestoneWithSaveChangesAsync(UserActivity userActivity, CancellationToken cancellationToken);
}
