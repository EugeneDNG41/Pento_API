using System.Threading;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Shared;
using Pento.Domain.UserEntitlements;
using Pento.Domain.UserSubscriptions;
using Quartz;
namespace Pento.Infrastructure.External.Quartz;
#pragma warning disable S125
[DisallowConcurrentExecution]
internal sealed class ProcessEntitlementResetJob(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<UserEntitlement> userEntitlementRepository,
    IUnitOfWork unitOfWork
    ) : IJob

{
    public async Task Execute(IJobExecutionContext context)
    {
        DateOnly today = dateTimeProvider.Today;
        var resettableEntitlements = (await userEntitlementRepository.FindAsync(e => e.ResetPeriod != null, context.CancellationToken)).ToList();
        foreach (UserEntitlement entitlement in resettableEntitlements)
        {
            if (entitlement.ResetPeriod == TimeUnit.Day)
            {
                entitlement.ResetUsage();
            }
            if (entitlement.ResetPeriod == TimeUnit.Week && today.DayOfWeek == DayOfWeek.Monday)
            {
                entitlement.ResetUsage();
            }
            if (entitlement.ResetPeriod == TimeUnit.Month && today.Day == 1)
            {
                entitlement.ResetUsage();
            }
            if (entitlement.ResetPeriod == TimeUnit.Year && today.Month == 1)
            {
                entitlement.ResetUsage();
            }
        }
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}

