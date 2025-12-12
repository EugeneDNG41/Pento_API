using System;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Converter;
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
    IConverterService converter,
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

        var ingredients = (await ingredientRepo.FindAsync(
            x => x.RecipeId == command.RecipeId,
            cancellationToken)).ToList();

        if (!ingredients.Any())
        {
            return Result.Failure<Guid>(RecipeErrors.NoIngredients);
        }

        var foodRefIds = ingredients.Select(i => i.FoodRefId).ToHashSet();

        var foodItems = (await foodItemRepo.FindAsync(
            x => x.HouseholdId == householdId.Value &&
                 foodRefIds.Contains(x.FoodReferenceId),
            cancellationToken)).ToList();

        if (!foodItems.Any())
        {
            return Result.Failure<Guid>(FoodItemErrors.NotFound);
        }

        var foodItemIds = foodItems.Select(f => f.Id).ToHashSet();

        var reservations = (await reservationRepo.FindAsync(
            x => x.MealPlanId == command.MealPlanId &&
                 foodItemIds.Contains(x.FoodItemId),
            cancellationToken)).ToList();

        if (!reservations.Any())
        {
            return Result.Failure<Guid>(FoodItemReservationErrors.NotFound);
        }

        if (reservations.Count != ingredients.Count)
        {
            return Result.Failure<Guid>(FoodItemReservationErrors.MismatchWithIngredients);
        }

        foreach (FoodItemMealPlanReservation? r in reservations)
        {
            if (r.Status != ReservationStatus.Pending)
            {
                return Result.Failure<Guid>(FoodItemReservationErrors.InvalidState);
            }

            FoodItem foodItem = foodItems.Single(fi => fi.Id == r.FoodItemId);
            Result<decimal> qtyInItemUnit = await converter.ConvertAsync(
                r.Quantity,
                r.UnitId,
                foodItem.UnitId,
                cancellationToken
            );
            if (qtyInItemUnit.IsFailure)
            {
                return Result.Failure<Guid>(qtyInItemUnit.Error);
            }
            foodItem.AdjustReservedQuantity(qtyInItemUnit.Value);

            r.MarkAsCancelled();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return command.RecipeId;
    }
}


