using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;
using Pento.Domain.Recipes;

namespace Pento.Application.MealPlans.Create;

internal sealed class CreateMealPlanCommandHandler(
    IMealPlanRepository mealPlanRepository,
    IGenericRepository<Recipe> recipeRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateMealPlanCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateMealPlanCommand command, CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;

        if (householdId is null)
        {
            return Result.Failure<Guid>(MealPlanErrors.ForbiddenAccess);
        }
        Recipe? recipe = await recipeRepository.GetByIdAsync(command.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return Result.Failure<Guid>(RecipeErrors.NotFound);
        }
        MealPlan? existingPlan = await mealPlanRepository.GetByNameAsync(command.Name, cancellationToken);
        if (existingPlan is not null)
        {
            return Result.Failure<Guid>(MealPlanErrors.DuplicateName);
        }
    

        DateTime utcNow = DateTime.UtcNow;

        var mealPlan = MealPlan.Create(
            householdId.Value,
            command.RecipeId,
            command.Name,
            command.MealType,
            command.ScheduledDate,
            command.Servings,
            command.Notes,
            userContext.UserId,
            utcNow
        );

        await mealPlanRepository.AddAsync(mealPlan, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(mealPlan.Id);
    }
}
