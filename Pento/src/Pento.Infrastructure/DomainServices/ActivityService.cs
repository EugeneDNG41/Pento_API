using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.DomainServices;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.UserActivities;

namespace Pento.Infrastructure.DomainServices;

internal sealed class ActivityService(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<Activity> activityRepostiory,
    IGenericRepository<UserActivity> userActivityRepository) : IActivityService
{
    public async Task<Result<UserActivity>> RecordActivityAsync(Guid userId, string activityCode, CancellationToken cancellationToken)
    {
        Activity? activity = (await activityRepostiory.FindAsync(a => a.Code == activityCode, cancellationToken)).SingleOrDefault();
        if (activity is null)
        {
            return Result.Failure<UserActivity>(ActivityErrors.NotFound);
        }
        var userActivity = UserActivity.Create(userId, activity.Code, dateTimeProvider.UtcNow);
        userActivityRepository.Add(userActivity);
        return userActivity;
    }
}
