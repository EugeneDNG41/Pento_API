using Pento.Domain.Abstractions;
using Pento.Domain.UserActivities;

namespace Pento.Application.Abstractions.Services;

public interface IActivityService
{
    Task<Result<int>> CountActivityAsync(Guid userId, string activityCode, CancellationToken cancellationToken);
    Task<Result<int>> CountMostWithinTimeAsync(Guid userId, string activityCode, TimeSpan withinTime, CancellationToken cancellationToken);
    Task<Result<UserActivity>> RecordActivityAsync(Guid userId, Guid? householdId, string activityCode, Guid? entityId, CancellationToken cancellationToken);
    Task<Result> RecordHouseholdActivityAsync(Guid householdId, string activityCode, Guid? entityId, CancellationToken cancellationToken);
}
