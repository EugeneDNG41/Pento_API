using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.UtilityServices.Clock;
using Pento.Application.Abstractions.UtilityServices.Converter;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Storages;
using Pento.Domain.Units;

namespace Pento.Infrastructure.UtilityServices.Converter;

internal sealed class ConverterService(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<Unit> unitRepository) : IConverterService
{
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
        DateOnly today = dateTimeProvider.Today;

        if (oldType == newType)
        {
            return currentExpiry;
        }

        double totalDaysOld = GetShelfLifeDays(oldType, foodRef);
        double totalDaysNew = GetShelfLifeDays(newType, foodRef);

        if (totalDaysNew <= 0)
        {
            return today;
        }

        if (totalDaysOld <= 0)
        {
            return today.AddDays((int)totalDaysNew);
        }

        int remainingDaysOld = currentExpiry.DayNumber - today.DayNumber;

        double remainingFraction = (double)remainingDaysOld / totalDaysOld;

        double newRemainingDays = totalDaysNew * remainingFraction;

        int newRemainingDaysRounded = (int)Math.Round(newRemainingDays);

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
