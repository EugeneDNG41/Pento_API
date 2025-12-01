using System.Linq.Expressions;
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
    public async Task<Result<UserActivity>> RecordActivityAsync(Guid userId, string activityCode, CancellationToken cancellationToken, Guid? entityId = null)
    {
        Activity? activity = (await activityRepostiory.FindAsync(a => a.Code == activityCode, cancellationToken)).SingleOrDefault();
        if (activity is null)
        {
            return Result.Failure<UserActivity>(ActivityErrors.NotFound);
        }
        var userActivity = UserActivity.Create(userId, activity.Code, dateTimeProvider.UtcNow, entityId);
        userActivityRepository.Add(userActivity);
        return userActivity;
    }
    public async Task<Result<int>> CountActivityAsync(Guid userId, string activityCode, DateTime? fromDate, CancellationToken cancellationToken)
    {
        Activity? activity = (await activityRepostiory.FindAsync(a => a.Code == activityCode, cancellationToken)).SingleOrDefault();
        if (activity is null)
        {
            return Result.Failure<int>(ActivityErrors.NotFound);
        }
        Expression<Func<UserActivity, bool>> predicate = 
            ua => ua.UserId == userId && 
            ua.ActivityCode == activityCode &&
            fromDate == null ||
            ua.PerformedOn >= fromDate;
        int activityCount = activity.Type switch
        {
            ActivityType.Action => await userActivityRepository.CountAsync(predicate, cancellationToken),
            ActivityType.State => await userActivityRepository.CountDistinctAsync(ua => ua.EntityId, predicate, cancellationToken),
            _ => 0
        };
        return activityCount;
    }
}
