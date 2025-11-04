using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;
using Pento.Domain.Recipes;

namespace Pento.Application.MealPlans.Create;

internal sealed class CreateMealPlanCommandHandler(
    IMealPlanRepository mealPlanRepository,
    IGenericRepository<Recipe> recipeRepository,
    IGenericRepository<Household> householeRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateMealPlanCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateMealPlanCommand command, CancellationToken cancellationToken)
    {
        Recipe? recipe = await recipeRepository.GetByIdAsync(command.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return Result.Failure<Guid>(RecipeErrors.NotFound);
        }
        Household? household = await householeRepository.GetByIdAsync(command.HouseholdId, cancellationToken);
        if (household is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotFound);
        }
        MealPlan? existingPlan = await mealPlanRepository.GetByNameAsync(command.HouseholdId, command.Name, cancellationToken);
        if (existingPlan is not null)
        {
            return Result.Failure<Guid>(MealPlanErrors.DuplicateName);
        }

        DateTime utcNow = DateTime.UtcNow;

        var mealPlan = MealPlan.Create(
            command.HouseholdId,
            command.RecipeId,
            command.Name,
            command.MealType,
            command.ScheduledDate,
            command.Servings,
            command.Notes,
            command.CreatedBy,
            utcNow
        );

        await mealPlanRepository.AddAsync(mealPlan, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(mealPlan.Id);
    }
}
