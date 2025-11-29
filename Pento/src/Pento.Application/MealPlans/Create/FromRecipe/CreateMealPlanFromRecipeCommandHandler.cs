using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItemReservations;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.MealPlanRecipe;
using Pento.Domain.MealPlans;
using Pento.Domain.RecipeIngredients;
using Pento.Domain.Recipes;
using Pento.Domain.Units;

namespace Pento.Application.MealPlans.Create.FromRecipe;
internal sealed class CreateMealPlanFromRecipeCommandHandler(
    IGenericRepository<Recipe> recipeRepo,
    IGenericRepository<FoodItem> foodItemRepo,
    IGenericRepository<RecipeIngredient> ingredientRepo,
    IGenericRepository<MealPlan> mealPlanRepo,
    IGenericRepository<FoodReference> foodRefRepo,
    IGenericRepository<Unit> unitRepo,
    IGenericRepository<MealPlanRecipe> mealPlanRecipeRepo,
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


        var ingredients = (await ingredientRepo.FindAsync(
            x => x.RecipeId == cmd.RecipeId,
            cancellationToken)).ToList();

        var reserved = new List<ReservationResult>();
        var missing = new List<MissingIngredientResult>();

        foreach (RecipeIngredient ingredient in ingredients)
        {
            FoodReference? foodRef = await foodRefRepo.GetByIdAsync(ingredient.FoodRefId, cancellationToken);

            string name = ingredient.Notes
                         ?? foodRef?.Name
                         ?? "Unknown ingredient";

            FoodItem? foodItem = (await foodItemRepo.FindAsync(
                x => x.HouseholdId == householdId.Value &&
                     x.FoodReferenceId == ingredient.FoodRefId,
                cancellationToken)).FirstOrDefault();
            Unit? ingredientUnit = await unitRepo.GetByIdAsync(ingredient.UnitId, cancellationToken);
            if (ingredientUnit is null)
            {
                return Result.Failure<MealPlanAutoReserveResult>(
                    UnitErrors.NotFound
                );
            }


            Unit? foodItemUnit = null;
            if (foodItem is not null)
            {
                foodItemUnit = await unitRepo.GetByIdAsync(foodItem.UnitId, cancellationToken);
                if (foodItemUnit is null)
                {
                    return Result.Failure<MealPlanAutoReserveResult>(
                        UnitErrors.NotFound
                    );
                }
            }


            if (foodItem is null)
            {
                missing.Add(new MissingIngredientResult(
                    ingredient.Id,
                    ingredient.FoodRefId,
                    name,
                    ingredient.Quantity,
                    ingredient.UnitId,
                    ingredientUnit.Abbreviation
                ));
                continue;
            }

            decimal requiredQty = ingredient.Quantity;

            if (ingredient.UnitId != foodItem.UnitId)
            {
                Result<decimal> converted = await converter.ConvertAsync(
                    ingredient.Quantity,
                    ingredient.UnitId,
                    foodItem.UnitId,
                    cancellationToken);

                if (converted.IsFailure)
                {
                    missing.Add(new MissingIngredientResult(
                        ingredient.Id,
                        ingredient.FoodRefId,
                        name,
                        ingredient.Quantity,
                        ingredient.UnitId,
                        ingredientUnit.Abbreviation
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
                    name,
                    ingredient.Quantity,
                    ingredient.UnitId,
                    ingredientUnit.Abbreviation

                ));
                continue;
            }

            reserved.Add(new ReservationResult(
                foodItem.Id,
                ingredient.Id,
                requiredQty,
                ingredient.Quantity,
                ingredient.UnitId,
                foodItem.UnitId,
                IngredientUnitAbbreviation: ingredientUnit.Abbreviation,
                FoodItemUnitAbbreviation: foodItemUnit!.Abbreviation 
            ));
        }

        if (missing.Count > 0)
        {
            return Result.Success(
                new MealPlanAutoReserveResult(Guid.Empty, reserved, missing)
            );
        }

        MealPlan? mealPlan = (await mealPlanRepo.FindAsync(
            x => x.HouseholdId == householdId.Value &&
                 x.ScheduledDate == cmd.ScheduledDate &&
                 x.MealType == cmd.MealType,
            cancellationToken)).FirstOrDefault();

        if (mealPlan is null)
        {
            string name = $"{cmd.ScheduledDate:yyyy-MM-dd} {cmd.MealType}";
            mealPlan = MealPlan.Create(
                householdId.Value,
                name,
                cmd.MealType,
                cmd.ScheduledDate,
                cmd.Servings,
                notes: null,
                createdBy: userContext.UserId,
                utcNow: clock.UtcNow
            );

            mealPlanRepo.Add(mealPlan);
        }

        var mealPlanRecipe = MealPlanRecipe.Create(mealPlan.Id, cmd.RecipeId);
        mealPlanRecipeRepo.Add(mealPlanRecipe);

        foreach (ReservationResult r in reserved)
        {
            FoodItem? foodItem = await foodItemRepo.GetByIdAsync(r.FoodItemId, cancellationToken);
            foodItem!.Reserve(r.ReservedQuantity, r.IngredientQuantity, r.IngredientUnitId, userContext.UserId);

            var resv = FoodItemMealPlanReservation.Create(
                r.FoodItemId,
                householdId.Value,
                clock.UtcNow,
                r.ReservedQuantity,
                r.FoodItemUnitId,
                mealPlan.Id
            );

            reservationRepo.Add(resv);
        }

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(
            new MealPlanAutoReserveResult(mealPlan.Id, reserved, missing)
        );
    }
}
