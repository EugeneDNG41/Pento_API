using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;
using Pento.Domain.Recipes;

namespace Pento.Application.MealPlans.Create;

internal sealed class CreateMealPlanCommandHandler(
    IGenericRepository<MealPlan> mealPlanRepository,
    IGenericRepository<Recipe> recipeRepository,
    IGenericRepository<FoodItem> foodItemRepository,
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
        if (command.RecipeId is not null)
        {
            Recipe? recipe = await recipeRepository.GetByIdAsync(command.RecipeId.Value, cancellationToken);
            if (recipe is null)
            {
                return Result.Failure<Guid>(RecipeErrors.NotFound);
            }
        }

        if (command.FoodItemId is not null)
        {
            FoodItem? foodItem = await foodItemRepository.GetByIdAsync(command.FoodItemId.Value, cancellationToken);
            if (foodItem is null)
            {
                return Result.Failure<Guid>(FoodItemErrors.NotFound);
            }
        }
        IEnumerable<MealPlan>? existingPlan = await mealPlanRepository.FindAsync(d => d.HouseholdId == householdId.Value && d.Name == command.Name, cancellationToken);
        if (existingPlan is not null)
        {
            return Result.Failure<Guid>(MealPlanErrors.DuplicateName);
        }
    

        DateTime utcNow = DateTime.UtcNow;

        var mealPlan = MealPlan.Create(
            householdId.Value,
            command.Name,
            command.MealType,
            command.ScheduledDate,
            command.Servings,
            command.Notes,
            userContext.UserId,
            utcNow
    );

        mealPlanRepository.Add(mealPlan);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(mealPlan.Id);
    }
}
