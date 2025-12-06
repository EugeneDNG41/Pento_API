using System.Linq.Expressions;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Services;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.UserActivities;

namespace Pento.Infrastructure.Services;

internal sealed class ActivityService(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<Activity> activityRepostiory,
    IGenericRepository<UserActivity> userActivityRepository) : IActivityService
{
    public async Task<Result<UserActivity>> RecordActivityAsync(Guid userId, Guid? householdId, string activityCode, Guid? entityId, CancellationToken cancellationToken)
    {
        Activity? activity = (await activityRepostiory.FindAsync(a => a.Code == activityCode, cancellationToken)).SingleOrDefault();
        if (activity is null)
        {
            return Result.Failure<UserActivity>(ActivityErrors.NotFound);
        }
        var userActivity = UserActivity.Create(userId, householdId, activity.Code, dateTimeProvider.UtcNow, entityId);
        userActivityRepository.Add(userActivity);
        return userActivity;
    }
    public async Task<Result<int>> CountActivityAsync(Guid userId, string activityCode, CancellationToken cancellationToken)
    {
        Activity? activity = (await activityRepostiory.FindAsync(a => a.Code == activityCode, cancellationToken)).SingleOrDefault();
        if (activity is null)
        {
            return Result.Failure<int>(ActivityErrors.NotFound);
        }
        Expression<Func<UserActivity, bool>> predicate =
            ua => ua.UserId == userId &&
            ua.ActivityCode == activityCode;
        return await userActivityRepository.CountAsync(predicate, cancellationToken);

    }
    public async Task<Result<int>> CountMostWithinTimeAsync(Guid userId, string activityCode, TimeSpan withinTime,  CancellationToken cancellationToken)
    {
        Expression<Func<UserActivity, bool>> predicate =
            ua => ua.UserId == userId &&
            ua.ActivityCode == activityCode;
        var activityTimestamps = (await userActivityRepository.FindAsync(predicate, cancellationToken)).Select(ua => ua.PerformedOn).OrderBy(dt => dt).ToList();

        int maxCount = 0;
        int right = 0;
        for (int left = 0; left < activityTimestamps.Count; left++)
        {
            DateTime leftTime = activityTimestamps[left];

            while (right < activityTimestamps.Count && activityTimestamps[right] <= leftTime + withinTime)
            {
                right++;
            }

            int countInWindow = right - left;
            if (countInWindow > maxCount)
            {
                maxCount = countInWindow;
            }
        }
        return maxCount;
    }
}
