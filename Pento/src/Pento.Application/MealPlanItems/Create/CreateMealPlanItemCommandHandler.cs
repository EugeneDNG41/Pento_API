using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlanItems;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlanItems.Create;
internal sealed class CreateMealPlanItemCommandHandler(
    IMealPlanItemRepository mealPlanItemRepository,
    IMealPlanRepository mealPlanRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateMealPlanItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateMealPlanItemCommand request,
        CancellationToken cancellationToken)
    {
        MealPlan mealPlan = await mealPlanRepository.GetByIdAsync(request.MealPlanId, cancellationToken);
        if (mealPlan is null)
        {
            return Result.Failure<Guid>(MealPlanItemErrors.NotFound(request.MealPlanId));
        }

        if (!Enum.TryParse<MealType>(request.MealType, true, out MealType mealType))
        {
            return Result.Failure<Guid>(MealPlanItemErrors.InvalidMealType);
        }

        if (request.Schedule is null || request.Schedule.Count == 0)
        {
            return Result.Failure<Guid>(MealPlanItemErrors.InvalidSchedule);
        }

        if (request.Servings <= 0)
        {
            return Result.Failure<Guid>(MealPlanItemErrors.InvalidServings);
        }

        DateTime utcNow = DateTime.UtcNow;

        var mealPlanItem = MealPlanItem.Create(
            request.MealPlanId,
            request.RecipeId,
            mealType,
            request.Schedule,
            request.Servings,
            request.Notes,
            utcNow
        );

        await mealPlanItemRepository.AddAsync(mealPlanItem, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(mealPlanItem.Id);
    }
}
