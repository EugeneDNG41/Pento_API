using Microsoft.Extensions.Logging;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.ThirdPartyServices.Firebase;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Application.Abstractions.UtilityServices.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.DeviceTokens;
using Pento.Domain.FoodItems;
using Pento.Domain.Notifications;
using Quartz;

namespace Pento.Infrastructure.ThirdPartyServices.Quartz;

[DisallowConcurrentExecution]
internal sealed class ProcessExpirationDateTrackingJob(
    IGenericRepository<FoodItem> foodItemRepository
) : IJob
{

    public async Task Execute(IJobExecutionContext context)
    {

        var foodItems = (await foodItemRepository.FindAsync(
            fi => fi.Quantity > 0,
            context.CancellationToken)).ToList();

        foreach (FoodItem? foodItem in foodItems)
        {          
            
           //dosth

        }
    }
}
