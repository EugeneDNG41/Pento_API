using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Application.Abstractions.Utility.Clock;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Storages;
using Pento.Domain.Users;

namespace Pento.Application.Households.Create;

internal sealed class CreateHouseholdCommandHandler(
    IDateTimeProvider dateTimeProvider,
    IUserContext userContext,
    IGenericRepository<Household> householdRepository,
    IGenericRepository<User> userRepository,
    IGenericRepository<Role> roleRepository,
    IGenericRepository<Storage> storageRepository,
    IGenericRepository<Compartment> compartmentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateHouseholdCommand, string>
{
    public async Task<Result<string>> Handle(CreateHouseholdCommand command, CancellationToken cancellationToken)
    {
        User? currentUser = (await userRepository.FindIncludeAsync(u => u.Id == userContext.UserId, u => u.Roles, cancellationToken)).SingleOrDefault();
        if (currentUser is null)
        {
            return Result.Failure<string>(UserErrors.NotFound);
        }
        //deal with previous household if exists
        Guid? previousHouseholdId = currentUser.HouseholdId;
        Role? householdHeadRole = (await roleRepository.FindAsync(r => r.Name == Role.HouseholdHead.Name, cancellationToken)).Single();
        if (previousHouseholdId is not null)
        {
            IEnumerable<User> otherMembers = await userRepository.FindIncludeAsync(
                u => u.HouseholdId == currentUser.HouseholdId && u.Id != currentUser.Id,
                u => u.Roles,
                cancellationToken);

            bool hasHead = otherMembers.Any(m =>
                m.Roles.Any(r => r.Name == householdHeadRole.Name));
            if (!hasHead && otherMembers.Any())
            {
                User newHead = otherMembers.First();
                newHead.SetRoles([householdHeadRole]);
            }
        }
        //new household
        var household = Household.Create(command.Name, dateTimeProvider.UtcNow, userContext.UserId);
        householdRepository.Add(household);

        currentUser.SetHouseholdId(household.Id);
        currentUser.SetRoles([householdHeadRole]);
        userRepository.Update(currentUser);

        var pantry = Storage.AutoCreate("Default Pantry", household.Id, StorageType.Pantry, null);
        var fridge = Storage.AutoCreate("Default Fridge", household.Id, StorageType.Fridge, null);
        var freezer = Storage.AutoCreate("Default Freezer", household.Id, StorageType.Freezer, null);
        storageRepository.AddRange([pantry, fridge, freezer]);
        var pantryCompartment = Compartment.AutoCreate("Default Pantry Compartment", pantry.Id, household.Id, null);
        var fridgeCompartment = Compartment.AutoCreate("Default Fridge Compartment", fridge.Id, household.Id, null);
        var freezerCompartment = Compartment.AutoCreate("Default Freezer Compartment", freezer.Id, household.Id, null);
        compartmentRepository.AddRange([pantryCompartment, fridgeCompartment, freezerCompartment]);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return household.InviteCode;
    }
}
