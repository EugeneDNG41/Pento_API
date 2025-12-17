using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.MealPlans;

namespace Pento.Application.MealPlans.Update;

internal sealed class UpdateMealPlanCommandHandler(
    IGenericRepository<MealPlan> mealPlanRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IDateTimeProvider datetimeProvider
) : ICommandHandler<UpdateMealPlanCommand>
{
    public async Task<Result> Handle(UpdateMealPlanCommand command, CancellationToken cancellationToken)
    {
        MealPlan? mealPlan = await mealPlanRepository.GetByIdAsync(command.Id, cancellationToken);
        if (mealPlan is null)
        {
            return Result.Failure(MealPlanErrors.NotFound);
        }
        if (mealPlan.HouseholdId != userContext.HouseholdId)
        {
            return Result.Failure(MealPlanErrors.ForbiddenAccess);
        }
        mealPlan.Update(
            command.MealType,
            command.ScheduledDate,
            command.Servings,
            command.Notes,
            datetimeProvider.UtcNow
            );

        await mealPlanRepository.UpdateAsync(mealPlan, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
