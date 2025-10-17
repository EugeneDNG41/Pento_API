using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.PossibleUnits;
public interface IPossibleUnitRepository
{
    Task<IReadOnlyList<PossibleUnit>> GetByFoodRefIdAsync(
        Guid foodRefId,
        CancellationToken cancellationToken = default
    );

    Task<PossibleUnit?> GetDefaultForFoodRefAsync(
        Guid foodRefId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(PossibleUnit possibleUnit, CancellationToken cancellationToken = default);

    Task UpdateAsync(PossibleUnit possibleUnit, CancellationToken cancellationToken = default);
}
