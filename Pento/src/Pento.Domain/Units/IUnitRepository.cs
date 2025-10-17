using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.Units;
public interface IUnitRepository
{
    Task<Unit?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Unit?> GetByAbbreviationAsync(string abbreviation, CancellationToken cancellationToken = default);
    Task AddAsync(Unit unit, CancellationToken cancellationToken = default);
}
