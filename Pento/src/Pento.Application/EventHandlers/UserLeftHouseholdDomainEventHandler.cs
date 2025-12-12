using Pento.Application.Abstractions.Exceptions;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Trades;
using Pento.Domain.Users;
using Pento.Domain.Users.Events;

namespace Pento.Application.EventHandlers;

internal sealed class UserLeftHouseholdDomainEventHandler(
    IGenericRepository<Household> householdRepository,
    IGenericRepository<User> repository,
    IUnitOfWork unitOfWork) : DomainEventHandler<UserLeftHouseholdDomainEvent>
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
        else if (!users.Any())
        {
            Household? household = await householdRepository.GetByIdAsync(domainEvent.HouseholdId, cancellationToken);
            if (household == null)
            {
                throw new PentoException(nameof(UserLeftHouseholdDomainEventHandler), HouseholdErrors.NotFound);
            }
            household.Delete();
            householdRepository.Update(household);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }
        
    }
}
