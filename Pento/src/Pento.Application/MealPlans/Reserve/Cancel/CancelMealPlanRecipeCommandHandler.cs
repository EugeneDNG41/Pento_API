using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.MealPlanRecipe;
using Pento.Domain.MealPlans;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Recipes;

namespace Pento.Application.MealPlans.Reserve.Cancel;
internal sealed class CancelMealPlanRecipeCommandHandler(
    IGenericRepository<RecipeIngredient> ingredientRepo,
    IGenericRepository<FoodItem> foodItemRepo,
    IGenericRepository<FoodItemMealPlanReservation> reservationRepo,
    IGenericRepository<MealPlanRecipe> mealPlanRecipeRepo,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<CancelMealPlanRecipeCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CancelMealPlanRecipeCommand command,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<Guid>(HouseholdErrors.NotInAnyHouseHold);
        }

        bool exists = (await mealPlanRecipeRepo.FindAsync(
            x => x.MealPlanId == command.MealPlanId &&
                 x.RecipeId == command.RecipeId,
            cancellationToken
        )).Any();

        if (!exists)
        {
            return Result.Failure<Guid>(MealPlanErrors.RecipeNotInMealPlan);
        }

        IEnumerable<RecipeIngredient> ingredients = await ingredientRepo.FindAsync(
            x => x.RecipeId == command.RecipeId,
            cancellationToken
        );

        if (!ingredients.Any())
        {
            return Result.Failure<Guid>(RecipeErrors.NoIngredients);
        }

        var foodRefIds = ingredients.Select(i => i.FoodRefId).ToList();

        IEnumerable<FoodItem> foodItems = await foodItemRepo.FindAsync(
            x => x.HouseholdId == householdId.Value &&
                 foodRefIds.Contains(x.FoodReferenceId),
            cancellationToken
        );

        if (!foodItems.Any())
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }

        var foodItemIds = foodItems.Select(f => f.Id).ToList();

        IEnumerable<FoodItemMealPlanReservation> reservations = await reservationRepo.FindAsync(
            x => x.MealPlanId == command.MealPlanId &&
                 foodItemIds.Contains(x.FoodItemId),
            cancellationToken
        );

        if (!reservations.Any())
        {
            return Result.Failure<Guid>(FoodItemReservationErrors.NotFound);
        }

        foreach (FoodItemMealPlanReservation reservation in reservations.OfType<FoodItemMealPlanReservation>())
        {
            if (reservation.Status != ReservationStatus.Pending)
            {
                return Result.Failure<Guid>(FoodItemReservationErrors.InvalidState);
            }

            FoodItem? foodItem = foodItems.First(fi => fi.Id == reservation.FoodItemId);
            foodItem.AdjustQuantity(
                foodItem.Quantity + reservation.Quantity,
                userContext.UserId
            );

            reservation.MarkAsCancelled();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return command.RecipeId;
    }
}

