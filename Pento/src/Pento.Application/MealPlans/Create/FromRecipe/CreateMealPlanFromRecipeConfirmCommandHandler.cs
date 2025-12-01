using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Application.Abstractions.UtilityServices.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
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

internal sealed class CreateMealPlanFromRecipeConfirmCommandHandler(
    IGenericRepository<Recipe> recipeRepo,
    IGenericRepository<FoodItem> foodItemRepo,
    IGenericRepository<RecipeIngredient> ingredientRepo,
    IGenericRepository<MealPlan> mealPlanRepo,
    IGenericRepository<FoodReference> foodRefRepo,
    IGenericRepository<Unit> unitRepo,
    IGenericRepository<MealPlanRecipe> mealPlanRecipeRepo,
    IGenericRepository<FoodItemMealPlanReservation> reservationRepo,
    IGenericRepository<Compartment> compartmentRepo,
    IConverterService converter,
    IUserContext userContext,
    IDateTimeProvider clock,
    IUnitOfWork uow
) : ICommandHandler<CreateMealPlanFromRecipeConfirmCommand, MealPlanAutoReserveResult>
{
    public async Task<Result<MealPlanAutoReserveResult>> Handle(
        CreateMealPlanFromRecipeConfirmCommand cmd,
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

        (List<ReservationResult> reserved, List<MissingIngredientResult> missing) = await BuildReservationAsync(
            ingredients,
            householdId.Value,
            foodItemRepo,
            foodRefRepo,
            unitRepo,
            converter,
            cancellationToken
        );

        if (missing.Count > 0)
        {
            IEnumerable<Compartment> compartments = await compartmentRepo.FindAsync(
                c => c.HouseholdId == householdId.Value,
                cancellationToken);

            Compartment? defaultCompartment = compartments.FirstOrDefault();

            if (defaultCompartment is null)
            {
                return Result.Failure<MealPlanAutoReserveResult>(
                    CompartmentErrors.NotFound
                );
            }

            var today = DateOnly.FromDateTime(clock.UtcNow.Date);

            foreach (MissingIngredientResult m in missing)
            {
                FoodReference? foodRef = await foodRefRepo.GetByIdAsync(m.FoodRefId, cancellationToken);
                string name = m.Name ?? foodRef?.Name ?? "Unknown ingredient";

                var foodItem = FoodItem.Create(
                    foodReferenceId: m.FoodRefId,
                    compartmentId: defaultCompartment.Id,
                    householdId: householdId.Value,
                    name: name,
                    imageUrl: null,
                    quantity: m.RequiredQuantity,
                    unitId: m.UnitId,
                    expirationDate: today,
                    notes: $"Auto-created from missing ingredient of recipe {recipe.Title}",
                    addedBy: userContext.UserId
                );

                foodItemRepo.Add(foodItem);
            }

            await uow.SaveChangesAsync(cancellationToken);

            (reserved, missing) = await BuildReservationAsync(
                ingredients,
                householdId.Value,
                foodItemRepo,
                foodRefRepo,
                unitRepo,
                converter,
                cancellationToken
            );
        }

        if (missing.Count > 0)
        {
            return Result.Failure<MealPlanAutoReserveResult>(
                Error.Failure("STILL_MISSING", "Some ingredients are still missing after auto-adding.")
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
            foodItem!.Reserve(
                r.ReservedQuantity,
                r.IngredientQuantity,
                r.IngredientUnitId,
                userContext.UserId
            );

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


    private static async Task<(List<ReservationResult> Reserved, List<MissingIngredientResult> Missing)> BuildReservationAsync(
        List<RecipeIngredient> ingredients,
        Guid householdId,
        IGenericRepository<FoodItem> foodItemRepo,
        IGenericRepository<FoodReference> foodRefRepo,
        IGenericRepository<Unit> unitRepo,

        IConverterService converter,
        CancellationToken cancellationToken)
    {
        var reserved = new List<ReservationResult>();
        var missing = new List<MissingIngredientResult>();

        foreach (RecipeIngredient ingredient in ingredients)
        {
            FoodReference? foodRef = await foodRefRepo.GetByIdAsync(ingredient.FoodRefId, cancellationToken);

            string name =  foodRef?.Name
                         ?? "Unknown ingredient";

            FoodItem? foodItem = (await foodItemRepo.FindAsync(
                x => x.HouseholdId == householdId &&
                     x.FoodReferenceId == ingredient.FoodRefId,
                cancellationToken)).FirstOrDefault();

            Unit? ingredientUnit = await unitRepo.GetByIdAsync(ingredient.UnitId, cancellationToken);
            Unit? foodItemUnit = null;
            if (foodItem is not null)
            {
                foodItemUnit = await unitRepo.GetByIdAsync(foodItem.UnitId, cancellationToken);
            }

            if (foodItem is null)
            {
                missing.Add(new MissingIngredientResult(
                    ingredient.Id,
                    ingredient.FoodRefId,
                    name,
                    ingredient.Quantity,
                    ingredient.UnitId,
                    ingredientUnit?.Abbreviation ?? "un"
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
                        ingredientUnit?.Abbreviation ?? "un"
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
                    ingredientUnit?.Abbreviation ?? "un"

                ));
                continue;
            }
            if (foodItemUnit is null)
            {
                missing.Add(new MissingIngredientResult(
                    ingredient.Id,
                    ingredient.FoodRefId,
                    name,
                    ingredient.Quantity,
                    ingredient.UnitId,
                    ingredientUnit?.Abbreviation ?? "un"
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
                ingredientUnit?.Abbreviation ?? "un",
                foodItemUnit.Abbreviation

            ));
        }

        return (reserved, missing);
    }
}
