using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Application.Abstractions.Utility.Converter;
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

        Result<(List<ReservationResult>, List<MissingIngredientResult>)> reservation  = await BuildReservationAsync(
            ingredients,
            householdId.Value,
            cancellationToken
        );
        if (reservation.IsFailure)
        {
            return Result.Failure<MealPlanAutoReserveResult>(reservation.Error);
        }
        List<ReservationResult> reserved = reservation.Value.Item1;
        List<MissingIngredientResult> missing = reservation.Value.Item2;
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
                var foodItem = FoodItem.Create(
                    foodReferenceId: m.FoodRefId,
                    compartmentId: defaultCompartment.Id,
                    householdId: householdId.Value,
                    name: m.Name,
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
            reservation = await BuildReservationAsync(
            ingredients,
            householdId.Value,
            cancellationToken
                );

                    if (reservation.IsFailure)
                    {
                        return Result.Failure<MealPlanAutoReserveResult>(reservation.Error);
                    }

                    reserved = reservation.Value.Item1;
                    missing = reservation.Value.Item2;

                }
        if (missing.Count > 0)
        {
             return Result.Failure<MealPlanAutoReserveResult>(MealPlanErrors.CouldNotReserveAllIngredients);
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
                r.IngredientQuantity,
                r.IngredientUnitId,
                mealPlan.Id
            );

            reservationRepo.Add(resv);
        }

        await uow.SaveChangesAsync(cancellationToken);



        return Result.Success(
            new MealPlanAutoReserveResult(mealPlan.Id, reserved, missing)
        );
    }


    private async Task<Result<(List<ReservationResult> Reserved, List<MissingIngredientResult> Missing)>> BuildReservationAsync(
        List<RecipeIngredient> ingredients,
        Guid householdId,
        CancellationToken cancellationToken)
    {
        var reserved = new List<ReservationResult>();
        var missing = new List<MissingIngredientResult>();

        foreach (RecipeIngredient ingredient in ingredients)
        {
            FoodReference? foodRef = await foodRefRepo.GetByIdAsync(ingredient.FoodRefId, cancellationToken);

            if (foodRef is null)
            {
                return Result.Failure<(List<ReservationResult> Reserved, List<MissingIngredientResult> Missing)>(FoodReferenceErrors.NotFound);
            }

            var foodItems = (await foodItemRepo.FindAsync(
                x => x.HouseholdId == householdId &&
                     x.FoodReferenceId == ingredient.FoodRefId &&
                     x.Quantity > 0,
                cancellationToken)).ToList();

                    FoodItem? foodItem = foodItems
                        .OrderByDescending(x => x.Quantity)
                        .FirstOrDefault();


            Unit? ingredientUnit = await unitRepo.GetByIdAsync(ingredient.UnitId, cancellationToken);
            if (ingredientUnit is null)
            {
                return Result.Failure<(List<ReservationResult> Reserved, List<MissingIngredientResult> Missing)>(UnitErrors.NotFound);
            }
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
                    foodRef.Name,
                    ingredient.Quantity,
                    ingredient.UnitId,
                    ingredientUnit.Abbreviation
                ));
                continue;
            }
            if (ingredientUnit.Type != foodRef.UnitType)
            {
                return Result.Failure<(List<ReservationResult> Reserved, List<MissingIngredientResult> Missing)>(UnitErrors.InvalidConversion);
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
                    return Result.Failure<(List<ReservationResult> Reserved, List<MissingIngredientResult> Missing)>(converted.Error);
                }

                requiredQty = converted.Value;
            }

            if (requiredQty > foodItem.Quantity)
            {
                missing.Add(new MissingIngredientResult(
                    ingredient.Id,
                    ingredient.FoodRefId,
                    foodRef.Name,
                    ingredient.Quantity,
                    ingredient.UnitId,
                    ingredientUnit.Abbreviation

                ));
                continue;
            }
            if (foodItemUnit is null)
            {
                missing.Add(new MissingIngredientResult(
                    ingredient.Id,
                    ingredient.FoodRefId,
                    foodRef.Name,
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
                ingredientUnit.Abbreviation,
                foodItemUnit.Abbreviation

            ));
        }

        return (reserved, missing);
    }
}
