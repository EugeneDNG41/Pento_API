using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Create;
internal sealed class CreateMealPlanCommandHandler(
    IMealPlanRepository mealPlanRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateMealPlanCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateMealPlanCommand request,
        CancellationToken cancellationToken)
    {
        MealPlan existingMealPlan = await mealPlanRepository
            .GetByNameAsync(request.HouseholdId, request.Name, cancellationToken);

        if (existingMealPlan is not null)
        {
            return Result.Failure<Guid>(MealPlanErrors.DuplicateName);
        }


        var mealPlan = MealPlan.Create(
            request.HouseholdId,
            request.RecipeId,
            request.Name,
            request.MealType,
            request.ScheduledDate,
            request.Servings,
            request.Notes,
            request.CreatedBy,
            DateTime.UtcNow
        );

        await mealPlanRepository.AddAsync(mealPlan, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(mealPlan.Id);
    }
}
