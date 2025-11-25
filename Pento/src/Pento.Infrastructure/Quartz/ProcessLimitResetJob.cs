using System.Runtime.InteropServices;
using Dapper;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Domain.Abstractions;
using Pento.Domain.PointBalances.Events;
using Pento.Domain.PointBalances.Projections;
using Pento.Domain.PointCaps;
using Pento.Domain.Shared;
using Pento.Domain.UserEntitlements;
using Pento.Domain.UserSubscriptions;
using Quartz;
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable CS9113 // Sections of code should not be commented out
#pragma warning disable IDE0060 // Remove unused parameter
namespace Pento.Infrastructure.Quartz;

[DisallowConcurrentExecution]
internal sealed class ProcessLimitResetJob(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<PointCap> pointCapRepository,
    IGenericRepository<UserEntitlement> userEntitlementRepository,
    IGenericRepository<UserSubscription> userSubscriptionRepository,
    IUnitOfWork unitOfWork
    ) : IJob

{
    public async Task Execute(IJobExecutionContext context)
    {
        DateOnly today = dateTimeProvider.Today;
        await ExecuteEntitlementReset(today, context.CancellationToken);
        //await ExecutePointCapReset(today, context.CancellationToken);
    }


    private async Task ExecuteEntitlementReset(DateOnly today, CancellationToken cancellationToken)
    {
        var resettableEntitlements = (await userEntitlementRepository.FindAsync(e => e.Limit != null && e.Limit.ResetPer != null, cancellationToken)).ToList();
        foreach (UserEntitlement entitlement in resettableEntitlements)
        {
            UserSubscription? subscriptionInstance = (await userSubscriptionRepository
                .FindAsync(s => s.UserId == entitlement.UserId && s.Status == SubscriptionStatus.Active, cancellationToken))
                .LastOrDefault();
            if (entitlement.Limit!.ResetPer == TimeUnit.Day)
            {
                entitlement.ResetUsage();
            }
            if (entitlement.Limit!.ResetPer == TimeUnit.Week &&
                (subscriptionInstance == null && today.DayOfWeek == DayOfWeek.Monday ||
                subscriptionInstance != null && (today.DayNumber - subscriptionInstance.StartDate.DayNumber) % 7 == 0))
            {
                entitlement.ResetUsage();
            }
            if (entitlement.Limit!.ResetPer == TimeUnit.Month &&
                (subscriptionInstance == null && today.Day == 1 ||
                subscriptionInstance != null && (today.DayNumber - subscriptionInstance.StartDate.DayNumber) % 30 == 0))
            {
                entitlement.ResetUsage();
            }
            if (entitlement.Limit!.ResetPer == TimeUnit.Year &&
                (subscriptionInstance == null && today.Day == 1 && today.Month == 1  ||
                subscriptionInstance != null && (today.DayNumber - subscriptionInstance.StartDate.DayNumber) % 365 == 0))
            {
                entitlement.ResetUsage();
            }
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}


//    private async Task ExecutePointCapReset(DateTimeOffset today, CancellationToken cancellationToken)
//    {
        //IReadOnlyList<PointBalanceDetail> pointBalances = await session.Query
        //    <PointBalanceDetail>().ToListAsync(cancellationToken);
        //IReadOnlyList<PointCap> pointCaps = (await pointCapRepository.GetAllAsync(cancellationToken)).AsList();
        //foreach (PointBalanceDetail pointBalance in pointBalances)
        //{
        //    foreach (PointCap pointCap in pointCaps)
        //    {
        //        if (pointBalance.Categories.ContainsKey(pointCap.Category))
        //        {
        //            if (pointCap.Limit.ResetPeriod == Period.Daily)
        //            {
        //                session.Events.Append(pointBalance.Id, new PointCapReset(Period.Daily));
        //            }
        //            if (pointCap.Limit.ResetPeriod == Period.Weekly && today.DayOfWeek == DayOfWeek.Monday)
        //            {
        //                session.Events.Append(pointBalance.Id, new PointCapReset(Period.Weekly));
        //            }
        //            if (pointCap.Limit.ResetPeriod == Period.Monthly && today.Day == 1)
        //            {
        //                session.Events.Append(pointBalance.Id, new PointCapReset(Period.Monthly));
        //            }
        //        }
        //    }
        //}
//    }
//}
