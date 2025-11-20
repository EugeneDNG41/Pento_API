using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Storages;
using Pento.Domain.Units;

namespace Pento.Infrastructure.Converter;

internal sealed class ConverterService(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<Unit> unitRepository) : IConverterService
{
    private const int ExpiringSoonThresholdDays = 3;
    public FoodItemStatus FoodItemStatusCalculator(DateOnly expirationDate)
    {
        DateOnly today = dateTimeProvider.Today;
        if (expirationDate < today)
        {
            return FoodItemStatus.Expired;
        }
        else if (expirationDate <= today.AddDays(ExpiringSoonThresholdDays))
        {
            return FoodItemStatus.Expiring;
        }
        else
        {
            return FoodItemStatus.Available;
        }
    }
    public decimal Convert(decimal quantity, Unit fromUnit, Unit toUnit)
    {
        if (fromUnit.Id == toUnit.Id)
        {
            return quantity;
        }
        decimal convertedQuantity = quantity * fromUnit.ToBaseFactor / toUnit.ToBaseFactor;
        return convertedQuantity;
    }
    public async Task<Result<decimal>> ConvertAsync(decimal quantity, Guid fromUnitId, Guid toUnitId, CancellationToken cancellationToken)
    {
        if (fromUnitId == toUnitId)
        {
            return quantity;
        }
        Unit? fromUnit = await unitRepository.GetByIdAsync(fromUnitId, cancellationToken);
        Unit? toUnit = await unitRepository.GetByIdAsync(toUnitId, cancellationToken);
        //check if both have same base unit as well
        if (fromUnit is null || toUnit is null)
        {
            return Result.Failure<decimal>(UnitErrors.NotFound);
        }
        if (fromUnit.Type != toUnit.Type)
        {
            return Result.Failure<decimal>(UnitErrors.InvalidConversion);
        }
        decimal convertedQuantity = Convert(quantity, fromUnit, toUnit);
        return convertedQuantity;
    }

    public async Task<bool> CompareGreaterOrEqualAsync(decimal quantity, Guid fromUnitId, decimal compareQuantity, Guid toUnitId, CancellationToken cancellationToken)
    {
        Unit? fromUnit = await unitRepository.GetByIdAsync(fromUnitId, cancellationToken);
        Unit? toUnit = await unitRepository.GetByIdAsync(toUnitId, cancellationToken);
        if (fromUnit is null || toUnit is null || fromUnit.Type != toUnit.Type)
        {
            return false;
        }
        decimal convertedQuantity = Convert(quantity, fromUnit, toUnit);
        return convertedQuantity >= compareQuantity;
    }
    public DateOnly CalculateNewExpiryRemainingFraction(
        StorageType oldType,
        StorageType newType,
        FoodReference foodRef,
        DateOnly currentExpiry
    )
    {
        // Get "today" in UTC. All calculations are date-based.
        DateOnly today = dateTimeProvider.Today;

        // --- 1. Handle Trivial/Edge Cases ---

        // If the storage type isn't changing, the expiry date doesn't change.
        if (oldType == newType)
        {
            return currentExpiry;
        }

        // --- 2. Get Shelf Life Data ---

        // Get the total shelf life (in days) for the old and new types.
        double totalDaysOld = GetShelfLifeDays(oldType, foodRef);
        double totalDaysNew = GetShelfLifeDays(newType, foodRef);

        // If the new storage type is not applicable (e.g., moving ice cream
        // to the pantry), its shelf life is 0. It expires immediately.
        if (totalDaysNew <= 0)
        {
            return today;
        }

        // If the old storage type was not applicable (e.g., pantry for ice cream),
        // we can't calculate a "fraction" of its life. We must assume the new
        // expiry is just today + the new shelf life.
        if (totalDaysOld <= 0)
        {
            return today.AddDays((int)totalDaysNew);
        }

        // --- 3. Calculate Remaining Fraction ---

        // Calculate how many days of life were remaining in the *old* storage.
        int remainingDaysOld = currentExpiry.DayNumber - today.DayNumber;

        // Calculate the "fraction" of life remaining.
        double remainingFraction = (double)remainingDaysOld / totalDaysOld;

        // --- 4. Apply Fraction to New Shelf Life ---

        // Apply that same fraction to the *new* storage type's total shelf life.
        double newRemainingDays = totalDaysNew * remainingFraction;

        // Round to the nearest whole day.
        int newRemainingDaysRounded = (int)Math.Round(newRemainingDays);

        // The new expiry date is today + the new remaining days.
        DateOnly newExpiryDate = today.AddDays(newRemainingDaysRounded);

        return newExpiryDate;
    }

    private static int GetShelfLifeDays(StorageType type, FoodReference fr) => type switch
    {
        StorageType.Pantry => fr.TypicalShelfLifeDays_Pantry,
        StorageType.Fridge => fr.TypicalShelfLifeDays_Fridge,
        StorageType.Freezer => fr.TypicalShelfLifeDays_Freezer,
        _ => 0
    };
}
