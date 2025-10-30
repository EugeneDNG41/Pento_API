using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.Households;

public interface IHouseholdRepository
{
    Task<Household?> GetHouseholdByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Household?> GetHouseholdByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default);
    Task AddAsync(Household household, CancellationToken cancellationToken = default);
}
