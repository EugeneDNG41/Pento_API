using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Application.Households.Leave;

internal sealed class UserLeftHouseholdDomainEventHandler(IGenericRepository<User> repository, IUnitOfWork unitOfWork) : DomainEventHandler<UserLeftHouseholdDomainEvent>
{
    public override async Task Handle(UserLeftHouseholdDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        IEnumerable<User> users = await repository.FindIncludeAsync(u => u.HouseholdId == domainEvent.HouseholdId, u => u.Roles, cancellationToken: cancellationToken);
        if (users.Any() && !users.Any(u => u.Roles.Contains(Role.HouseholdHead)))
        {
            User? firstUser = users.FirstOrDefault();
            if (firstUser is not null)
            {
                firstUser.SetRoles(new List<Role> { Role.HouseholdHead });
                repository.Update(firstUser);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
