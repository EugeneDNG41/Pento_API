using System.Runtime.InteropServices;
using Dapper;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Data;
using Pento.Domain.PointBalances.Events;
using Pento.Domain.PointBalances.Projections;
using Pento.Domain.PointCaps;
using Pento.Domain.Shared;
using Pento.Domain.UserSubscriptions;
using Quartz;
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable CS9113 // Sections of code should not be commented out
#pragma warning disable IDE0060 // Remove unused parameter
//namespace Pento.Infrastructure.Quartz;
//[DisallowConcurrentExecution]
//internal sealed class ProcessLimitResetJob(
//    IDateTimeProvider dateTimeProvider,
//    IGenericRepository<PointCap> pointCapRepository
//    ) : IJob

//{
//    public async Task Execute(IJobExecutionContext context)
//    {
//        DateTimeOffset now = dateTimeProvider.UtcNowOffset;
//        await ExecuteConsumableReset(now, context.CancellationToken);
//        await ExecutePointCapReset(now, context.CancellationToken);
//    }


//    private Task ExecuteConsumableReset(DateTimeOffset now, CancellationToken cancellationToken)

//    {
//        //IReadOnlyList<UserSubscriptionInstance> subscriptionInstances = await session.Query
//        //    <UserSubscriptionInstance>()
//        //    .Where(x => x.Status == SubscriptionStatus.Active)
//        //    .ToListAsync(cancellationToken);
//        //foreach (UserSubscriptionInstance subscriptionInstance in subscriptionInstances)
//        //{
//        //    foreach (Guid featureId in subscriptionInstance.Consumables.Keys)
//        //    {
//        //        if (subscriptionInstance.Consumables[featureId].ResetPeriod == Period.Daily)
//        //        {
//        //            session.Events.Append(subscriptionInstance.Id, new ConsumableReset(featureId));
//        //        }
//        //        if (subscriptionInstance.Consumables[featureId].ResetPeriod == Period.Weekly && now.Subtract(subscriptionInstance.StartDateUtc).Days % 7 == 0)
//        //        {
//        //            session.Events.Append(subscriptionInstance.Id, new ConsumableReset(featureId));
//        //        }
//        //        if (subscriptionInstance.Consumables[featureId].ResetPeriod == Period.Monthly && now.Subtract(subscriptionInstance.StartDateUtc).Days % 30 == 0)
//        //        {
//        //            session.Events.Append(subscriptionInstance.Id, new ConsumableReset(featureId));
//        //        }
//        //    }
//        //}
//    }


//    private async Task ExecutePointCapReset(DateTimeOffset now, CancellationToken cancellationToken)
//    {
//        //IReadOnlyList<PointBalanceDetail> pointBalances = await session.Query
//        //    <PointBalanceDetail>().ToListAsync(cancellationToken);
//        //IReadOnlyList<PointCap> pointCaps = (await pointCapRepository.GetAllAsync(cancellationToken)).AsList();
//        //foreach (PointBalanceDetail pointBalance in pointBalances)
//        //{
//        //    foreach (PointCap pointCap in pointCaps)
//        //    {
//        //        if (pointBalance.Categories.ContainsKey(pointCap.Category))
//        //        {
//        //            if (pointCap.Limit.ResetPeriod == Period.Daily)
//        //            {
//        //                session.Events.Append(pointBalance.Id, new PointCapReset(Period.Daily));
//        //            }
//        //            if (pointCap.Limit.ResetPeriod == Period.Weekly && now.DayOfWeek == DayOfWeek.Monday)
//        //            {
//        //                session.Events.Append(pointBalance.Id, new PointCapReset(Period.Weekly));
//        //            }
//        //            if (pointCap.Limit.ResetPeriod == Period.Monthly && now.Day == 1)
//        //            {
//        //                session.Events.Append(pointBalance.Id, new PointCapReset(Period.Monthly));
//        //            }
//        //        }
//        //    }
//        //}
//    }
//}
