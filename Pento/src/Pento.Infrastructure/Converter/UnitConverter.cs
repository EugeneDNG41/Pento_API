using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Converter;
using Pento.Application.Abstractions.Data;
using Pento.Domain.Abstractions;
using Pento.Domain.Units;

namespace Pento.Infrastructure.Converter;

internal sealed class UnitConverter(IGenericRepository<Unit> unitRepository) : IUnitConverter
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
}
