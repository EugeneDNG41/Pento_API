using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Storages;
using Pento.Domain.Units;

namespace Pento.Infrastructure.Converter;

internal sealed class Calculator(
    IDateTimeProvider dateTimeProvider,
    IGenericRepository<Unit> unitRepository) : ICalculator
{
    private const double ShelfLifeDampingK = 0.85;
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
    public DateTime CalculateNewExpiryRemainingFraction(
        DateTime lastPlacedAtUtc,             // when it was placed into the *old* storage type
        StorageType oldType,                  // current/origin storage type (before the move)
        StorageType newType,                  // destination storage type (after the move)
        FoodReference foodRef,                // typical shelf lives per storage type
        DateTime currentExpiryUtc,           // current expiry (optional; used as an upper cap)
        bool capAtCurrentExpiry = false        // true => never extend beyond currentExpiryUtc
    )
    {
        DateTime nowUtc = dateTimeProvider.UtcNow;
        int tOld = TypicalDays(oldType, foodRef);
        int tNew = TypicalDays(newType, foodRef);

        // Elapsed time spent in the *old* storage type since it was last placed there.
        // (If you have exact spans, pass the start of the current span as lastPlacedAtUtc.)
        double elapsedDays = Math.Max(0.0, (nowUtc - lastPlacedAtUtc).TotalDays);

        // Remaining fraction in old storage
        // r = 1 - (elapsed / T_old), clamped to [0, 1].
        double r = Math.Max(0.0, 1.0 - elapsedDays / tOld);

        // Mildly dampen upgrades so you don’t “almost reset” when moving to colder storage late.
        bool isUpgrade =
            oldType == StorageType.Pantry && (newType == StorageType.Fridge || newType == StorageType.Freezer) ||
            oldType == StorageType.Fridge && newType == StorageType.Freezer;

        double adjustedR = isUpgrade ? Math.Pow(r, Math.Clamp(ShelfLifeDampingK, 0.1, 1.0)) : r;

        // Remaining time in the new storage
        double remainNewDays = adjustedR * tNew;

        // Proposed new expiry
        DateTime proposedExpiry = nowUtc.AddDays(remainNewDays);

        // Apply safety caps (never go past Use-By; optionally never extend past current expiry)
        return Cap(proposedExpiry, currentExpiryUtc, capAtCurrentExpiry);
    }

    private static int TypicalDays(StorageType type, FoodReference fr) => type switch
    {
        StorageType.Pantry => fr.TypicalShelfLifeDays_Pantry,
        StorageType.Fridge => fr.TypicalShelfLifeDays_Fridge,
        StorageType.Freezer => fr.TypicalShelfLifeDays_Freezer,
        _ => 0
    };

    private static DateTime Cap(
        DateTime proposed,
        DateTime currentExpiryUtc,
        bool capAtCurrentExpiry)
    {
        DateTime capped = proposed;

        if (capAtCurrentExpiry && currentExpiryUtc is DateTime ce)
        {
            capped = Min(capped, ce);
        }

        return capped;
    }

    private static DateTime Min(DateTime a, DateTime b) => a <= b ? a : b;
}
