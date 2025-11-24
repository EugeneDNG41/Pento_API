using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.Households;
using Pento.Domain.MealPlans;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Recipes;

namespace Pento.Application.MealPlans.Create.From_Recipe;
internal sealed class CreateMealPlanFromRecipeCommandHandler(
    IGenericRepository<Recipe> recipeRepo,
    IGenericRepository<FoodItem> foodItemRepo,
    IGenericRepository<RecipeIngredient> ingredientRepo,
    IGenericRepository<MealPlan> mealPlanRepo,
    IGenericRepository<FoodItemMealPlanReservation> reservationRepo,
    IConverterService converter,
    IUserContext userContext,
    IDateTimeProvider clock,
    IUnitOfWork uow
) : ICommandHandler<CreateMealPlanFromRecipeCommand, MealPlanAutoReserveResult>
{
    public async Task<Result<MealPlanAutoReserveResult>> Handle(
        CreateMealPlanFromRecipeCommand cmd,
        CancellationToken cancellationToken)
    {
        Guid? householdId = userContext.HouseholdId;
        if (householdId is null)
        {
            return Result.Failure<MealPlanAutoReserveResult>(HouseholdErrors.NotInAnyHouseHold);
        }

        Recipe? recipe = await recipeRepo.GetByIdAsync(cmd.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return Result.Failure<MealPlanAutoReserveResult>(RecipeErrors.NotFound);
        }



        MealPlan mealPlan;

            string generatedName = $"{cmd.ScheduledDate:yyyy-MM-dd} {cmd.MealType}";

            mealPlan = MealPlan.Create(
                householdId.Value,
                name: generatedName,
                mealType: cmd.MealType,
                scheduledDate: cmd.ScheduledDate,
                servings: cmd.Servings,
                notes: null,
                createdBy: userContext.UserId,
                utcNow: clock.UtcNow
            );

            mealPlanRepo.Add(mealPlan);
        


        var ingredients = (await ingredientRepo.FindAsync(
            x => x.RecipeId == cmd.RecipeId,
            cancellationToken
        )).ToList();

        var reserved = new List<ReservationResult>();
        var missing = new List<MissingIngredientResult>();

        foreach (RecipeIngredient? ingredient in ingredients)
        {
            FoodItem? foodItem = (await foodItemRepo.FindAsync(
                x => x.HouseholdId == householdId.Value
                  && x.FoodReferenceId == ingredient.FoodRefId,
                cancellationToken
            )).FirstOrDefault();

            if (foodItem is null)
            {
                missing.Add(new MissingIngredientResult(
                    ingredient.Id,
                    ingredient.FoodRefId,
                    ingredient.Notes ?? "Unknown Ingredient",
                    ingredient.Quantity,
                    ingredient.UnitId
                ));
                continue;
            }

            decimal requiredQty = ingredient.Quantity;

            if (ingredient.UnitId != foodItem.UnitId)
            {
                Result<decimal> converted = await converter.ConvertAsync(
                    ingredient.Quantity, ingredient.UnitId, foodItem.UnitId, cancellationToken
                );

                if (converted.IsFailure)
                {
                    missing.Add(new MissingIngredientResult(
                        ingredient.Id,
                        ingredient.FoodRefId,
                        ingredient.Notes ?? "Unknown Ingredient",
                        ingredient.Quantity,
                        ingredient.UnitId
                    ));
                    continue;
                }

                requiredQty = converted.Value;
            }

            if (requiredQty > foodItem.Quantity)
            {
                missing.Add(new MissingIngredientResult(
                    ingredient.Id,
                    ingredient.FoodRefId,
                    ingredient.Notes ?? "Unknown Ingredient",
                    ingredient.Quantity,
                    ingredient.UnitId
                ));
                continue;
            }

            reserved.Add(new ReservationResult(
                foodItem.Id,
                ingredient.Id,
                requiredQty,
                ingredient.Quantity,
                ingredient.UnitId,
                foodItem.UnitId
            ));
        }

        if (missing.Count > 0)
        {
            return Result.Success(
                new MealPlanAutoReserveResult(
                    MealPlanId: mealPlan.Id,
                    Reservations: reserved,
                    Missing: missing
                )
            );
        }

        foreach (ReservationResult r in reserved)
        {
            FoodItem? foodItem = await foodItemRepo.GetByIdAsync(r.FoodItemId, cancellationToken);

            foodItem!.Reserve(
                r.ReservedQuantity,
                r.IngredientQuantity,
                r.IngredientUnitId,
                userContext.UserId
            );

            var reservation = FoodItemMealPlanReservation.Create(
                r.FoodItemId,
                householdId.Value,
                clock.UtcNow,
                r.ReservedQuantity,
                r.IngredientUnitId,
                mealPlan.Id
            );

            reservationRepo.Add(reservation);
        }

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(
            new MealPlanAutoReserveResult(
                MealPlanId: mealPlan.Id,
                Reservations: reserved,
                Missing: missing
            )
        );
    }
}

