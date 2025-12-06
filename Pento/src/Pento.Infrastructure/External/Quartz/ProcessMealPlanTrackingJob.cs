using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;
using Pento.Domain.Notifications;
using Pento.Infrastructure.External.Firebase;
using Quartz;

namespace Pento.Infrastructure.External.Quartz;
[DisallowConcurrentExecution]

internal sealed class ProcessMealPlanTrackingJob(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<MealPlan> mealplanRepository,
    INotificationService notificationService
    
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        DateOnly tomorrow = dateTimeProvider.Today.AddDays(1);

        IEnumerable<MealPlan> upcomingMealplan = await mealplanRepository.FindAsync(
            mp => mp.ScheduledDate == tomorrow,
            context.CancellationToken);

        if (!upcomingMealplan.Any())
        {
            return;
        }

        IEnumerable<Guid> householdIds = upcomingMealplan.Select(mp => mp.HouseholdId).Distinct();

        foreach (Guid householdId in householdIds)
        {
            var mealplansInHousehold = upcomingMealplan
                .Where(mp => mp.HouseholdId == householdId)
                .ToList();

            string title = "Upcoming Meal Plans";
            string body = $"You have {mealplansInHousehold.Count} meal plans scheduled for tomorrow:\n";

            var payload = new Dictionary<string, string>
            {
                { "householdId", householdId.ToString() }
            };

            foreach ((MealPlan mp, int idx) tuple in mealplansInHousehold.Select((mp, idx) => (mp, idx)))
            {
                payload.Add($"mealplan{tuple.idx + 1}", tuple.mp.Id.ToString());
            }

            Result result = await notificationService.SendToHouseholdAsync(
                householdId: householdId,
                title: title,
                body: body,
                NotificationType.MealPlanReminder,
                payload: payload,
                cancellationToken: context.CancellationToken
            );

            if (result.IsFailure)
            {
                throw new PentoException(nameof(ProcessMealPlanTrackingJob));
            }
        }
    }
}

