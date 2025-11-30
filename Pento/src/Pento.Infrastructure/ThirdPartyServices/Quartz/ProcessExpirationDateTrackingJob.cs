using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.UtilityServices.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Quartz;

namespace Pento.Infrastructure.ThirdPartyServices.Quartz;

[DisallowConcurrentExecution]
internal sealed class ProcessExpirationDateTrackingJob(
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : IJob // + subscription notification job
{
    public async Task Execute(IJobExecutionContext context)
    {
        var foodItems = (await foodItemRepository.FindAsync(fi => fi.Quantity > 0, context.CancellationToken)).ToList();
        foreach (FoodItem? foodItem in foodItems)
        {
            //do somthething
        }
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
