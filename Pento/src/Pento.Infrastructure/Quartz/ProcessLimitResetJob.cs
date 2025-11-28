using System.Runtime.InteropServices;
using Dapper;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;
using Pento.Domain.UserEntitlements;
using Pento.Domain.UserSubscriptions;
using Quartz;
namespace Pento.Infrastructure.Quartz;

[DisallowConcurrentExecution]
internal sealed class ProcessLimitResetJob(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<UserEntitlement> userEntitlementRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork
    ) : IJob

{
    public async Task Execute(IJobExecutionContext context)
    {
        DateOnly today = dateTimeProvider.Today;
        await ExecuteEntitlementReset(today, context.CancellationToken);
    }


    private async Task ExecuteEntitlementReset(DateOnly today, CancellationToken cancellationToken)
    {
        var resettableEntitlements = (await userEntitlementRepository.FindAsync(e => e.ResetPeriod != null, cancellationToken)).ToList();
        foreach (UserEntitlement entitlement in resettableEntitlements)
        {
            UserSubscription? subscriptionInstance = (await userSubscriptionRepository
                .FindAsync(s => s.UserId == entitlement.UserId && s.Status == SubscriptionStatus.Active, cancellationToken))
                .LastOrDefault();
            if (entitlement.ResetPeriod == TimeUnit.Day)
            {
                entitlement.ResetUsage();
            }
            if (entitlement.ResetPeriod == TimeUnit.Week &&
                (subscriptionInstance == null && today.DayOfWeek == DayOfWeek.Monday ||
                subscriptionInstance != null && (today.DayNumber - subscriptionInstance.StartDate.DayNumber) % 7 == 0))
            {
                entitlement.ResetUsage();
            }
            if (entitlement.ResetPeriod == TimeUnit.Month &&
                (subscriptionInstance == null && today.Day == 1 ||
                subscriptionInstance != null && (today.DayNumber - subscriptionInstance.StartDate.DayNumber) % 30 == 0))
            {
                entitlement.ResetUsage();
            }
            if (entitlement.ResetPeriod == TimeUnit.Year &&
                (subscriptionInstance == null && today.Day == 1 && today.Month == 1  ||
                subscriptionInstance != null && (today.DayNumber - subscriptionInstance.StartDate.DayNumber) % 365 == 0))
            {
                entitlement.ResetUsage();
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

