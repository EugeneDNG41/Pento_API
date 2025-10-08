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

        DateRange dateRange;
        try
        {
            dateRange = DateRange.Create(request.StartDate, request.EndDate);
        }
        catch (Exception)
        {
            return Result.Failure<Guid>(MealPlanErrors.InvalidDateRange);
        }

        DateTime utcNow = DateTime.UtcNow;

        var mealPlan = MealPlan.Create(
            request.HouseholdId,
            request.Name,
            request.CreatedBy,
            dateRange,
            utcNow);

        await mealPlanRepository.AddAsync(mealPlan, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(mealPlan.Id);
    }
}
