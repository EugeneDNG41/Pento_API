using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.Notifications;
using Quartz;

namespace Pento.Infrastructure.External.Quartz;

[DisallowConcurrentExecution]
internal sealed class ProcessExpirationDateTrackingJob(
    IDateTimeProvider dateTimeProvider,
    INotificationService notificationService,
    IGenericRepository<FoodItem> foodItemRepository
) : IJob
{

    public async Task Execute(IJobExecutionContext context)
    {
        DateOnly today = dateTimeProvider.Today;
        var expiringFoodItems = (await foodItemRepository.FindAsync(
            fi => fi.Quantity > 0 && fi.ExpirationDate == today.AddDays(1), //business rule: notify one day before expiration
            context.CancellationToken)).ToList();

        if (expiringFoodItems.Any())
        {
            IEnumerable<Guid> householdIds = expiringFoodItems.Select(fi => fi.HouseholdId).Distinct();
            foreach (Guid householdId in householdIds)
            {
                var itemsInHousehold = expiringFoodItems.Where(fi => fi.HouseholdId == householdId).ToList();
                string title = "Expiring Food Item(s)";
                string body = $"You have {itemsInHousehold.Count} food item(s) that may expire in 24hrs or less.";
                var payload = new Dictionary<string, string>()
                {
                    { "householdId", householdId.ToString() }
                };
                foreach (FoodItem? item in itemsInHousehold)
                {
                    payload.Add($"foodItemId{itemsInHousehold.IndexOf(item) + 1}", item.Id.ToString());
                }
                Result result = await notificationService.SendToHouseholdAsync(
                    householdId, title, body, NotificationType.FoodExpiringSoon, payload, context.CancellationToken);
                if (result.IsFailure)
                {
                    throw new PentoException(nameof(ProcessExpirationDateTrackingJob), result.Error);
                }
            }
        }
    }
}
