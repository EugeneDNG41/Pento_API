using System.Collections.Generic;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.DomainServices;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.Activities;
using Pento.Domain.Compartments;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Storages;
using Pento.Domain.Users;

namespace Pento.Application.Households.Create;

internal sealed class CreateHouseholdCommandHandler(
    IUserContext userContext,
    IActivityService activityService,
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
        Role? householdHeadRole = (await roleRepository.FindAsync(r => r.Name == Role.HouseholdHead.Name, cancellationToken)).Single();
        if (currentUser.HouseholdId is not null)
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
        var household = Household.Create(command.Name);
        householdRepository.Add(household);

        currentUser.SetHouseholdId(household.Id);       
        currentUser.SetRoles([householdHeadRole]);
        userRepository.Update(currentUser);

        var pantry = Storage.Create("Default Pantry", household.Id, StorageType.Pantry, null);
        var fridge = Storage.Create("Default Fridge", household.Id, StorageType.Fridge, null);
        var freezer = Storage.Create("Default Freezer", household.Id, StorageType.Freezer, null);
        storageRepository.AddRange([pantry, fridge, freezer]);
        var pantryCompartment = Compartment.Create("Default Pantry Compartment", pantry.Id, household.Id, null);
        var fridgeCompartment = Compartment.Create("Default Fridge Compartment", fridge.Id, household.Id, null);
        var freezerCompartment = Compartment.Create("Default Freezer Compartment", freezer.Id, household.Id, null);
        compartmentRepository.AddRange([pantryCompartment, fridgeCompartment, freezerCompartment]);
        await activityService.RecordActivityAsync(
            currentUser.Id,
            ActivityCode.HOUSEHOLD_CREATE.ToString(),
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return household.InviteCode;
    }
}
