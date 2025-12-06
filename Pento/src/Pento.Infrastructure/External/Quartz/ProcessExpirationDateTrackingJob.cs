using Pento.Application.Abstractions.External.Firebase;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.FoodItems;
using Quartz;

namespace Pento.Infrastructure.External.Quartz;

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
