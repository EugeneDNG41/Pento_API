using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Delete;
internal sealed class DeleteMealPlanCommandHandler(
    IGenericRepository<MealPlan> mealPlanRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<DeleteMealPlanCommand>
{
    public async Task<Result> Handle(DeleteMealPlanCommand command, CancellationToken cancellationToken)
    {
        MealPlan? mealPlan = await mealPlanRepository.GetByIdAsync(command.Id, cancellationToken);
        if (mealPlan is null)
        {
            return Result.Failure(MealPlanErrors.NotFound);
        }

        mealPlanRepository.Remove(mealPlan); //check if there's any reserved food
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
