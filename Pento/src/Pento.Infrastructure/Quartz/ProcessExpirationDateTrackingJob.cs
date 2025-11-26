using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Quartz;

namespace Pento.Infrastructure.Quartz;

[DisallowConcurrentExecution]
internal sealed class ProcessExpirationDateTrackingJob(
    IConverterService converterService,
    IGenericRepository<FoodItem> foodItemRepository,
    IUnitOfWork unitOfWork) : IJob // + subscription notification job
{
    public async Task Execute(IJobExecutionContext context)
    {
        var foodItems = (await foodItemRepository.FindAsync(fi => fi.Quantity > 0, context.CancellationToken)).ToList();
        foreach (FoodItem? foodItem in foodItems)
        {
            FoodItemStatus status = converterService.FoodItemStatusCalculator(foodItem.ExpirationDate);
            if (foodItem.Status != status)
            {
                foodItem.UpdateStatus(status);
                foodItemRepository.Update(foodItem);
            }//if expiring or expired, notify user? future enhancement
        }
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
