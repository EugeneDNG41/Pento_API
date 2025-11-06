using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Units;

namespace Pento.Application.Abstractions.Converter;

public interface IUnitConverter
{
    Task<Result<decimal>> ConvertAsync(decimal quantity, Guid fromUnitId, Guid toUnitId, CancellationToken cancellationToken);
    decimal Convert(decimal quantity, Unit fromUnit, Unit toUnit);
    Task<bool> CompareGreaterOrEqualAsync(decimal quantity, Guid fromUnitId, decimal compareQuantity, Guid toUnitId, CancellationToken cancellationToken);
}
