using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Pento.Application.Abstractions.External.Identity;
using Pento.Domain.Abstractions;
using Pento.Domain.Compartments;
using Pento.Domain.FoodItems;
using Pento.Domain.FoodReferences;
using Pento.Domain.Households;
using Pento.Domain.Roles;
using Pento.Domain.Storages;
using Pento.Domain.Units;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Persistence.Seed;
#pragma warning disable CA5394 // Do not use insecure randomness
internal sealed class DataSeeder(
    IIdentityProviderService identityProviderService,
    ApplicationDbContext dbContext)
{
    public async Task<Result<User>> SeedAdminAsync(CancellationToken cancellationToken = default)
    {
        User? adminUser = await dbContext.Set<User>().Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Roles.Any(r => r.Name == Role.Administrator.Name), cancellationToken);
        if (adminUser != null)
        {
            return adminUser;
        } 
        Role ? adminRole = await dbContext.Set<Role>()
            .FindAsync([Role.Administrator.Name], cancellationToken);

        if (adminRole == null)
        {
            return Result.Failure<User>(RoleErrors.NotFound);
        }
        string adminFirstName = "Admin";
        string adminLastName = "Pento";
        string adminEmail = "admin@pento.com";
        string adminPassword = "Admin123!";
        Result<string> adminIdentityId = await identityProviderService.RegisterUserAsync(
            new UserModel(adminEmail, adminPassword, adminFirstName, adminLastName),
            cancellationToken);
        if (adminIdentityId.IsFailure)
        {
            return Result.Failure<User>(adminIdentityId.Error);
        }
        adminUser = User.Create(
            email: adminEmail,
            firstName: adminFirstName,
            lastName: adminLastName,
            identityId: adminIdentityId.Value,
            createdAt: DateTime.UtcNow
        );
        adminUser.SetRoles(new[] { adminRole });
        dbContext.Set<User>().Add(adminUser);
        await dbContext.SaveChangesAsync(cancellationToken);
        return adminUser;
    }
    public async Task SeedRandomUsersAsync(CancellationToken cancellationToken = default)
    {
        var random = new Random();
        string[] firstNames =
        {
            "Alice", "Bob", "Charlie", "Diana", "Ethan",
            "Fiona", "George", "Hannah", "Ian", "Julia",
            "Kevin", "Laura", "Michael", "Nina", "Oliver",
            "Paula", "Quentin", "Rachel", "Samuel", "Tina",
            "Ulysses", "Victor", "Wendy", "Xavier", "Yvonne"
        };
        string[] lastNames =
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones",
            "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
            "Thomas", "Taylor", "Moore", "Jackson", "Martin",
            "Lee", "Perez", "Thompson", "White", "Harris"
        };
        
        var users = new List<User>();
        for (int i = 0; i < 50; i++)
        {
            string firstName = firstNames[random.Next(firstNames.Length)];
            string lastName = lastNames[random.Next(lastNames.Length)];
            string email = $"{firstName.ToLower(CultureInfo.InvariantCulture)}.{lastName.ToLower(CultureInfo.InvariantCulture)}@example.com";
            string password = "Password123!";
            Result<string> identityId = await identityProviderService.RegisterUserAsync(new UserModel(email, password, firstName, lastName), cancellationToken);
            if (identityId.IsFailure)
            {
                throw new InvalidOperationException($"Failed to register user {email}: {identityId.Error.Description}");
            }
            var user = User.Create(
                email: email,
                firstName: firstName,
                lastName: lastName,
                identityId: identityId.Value,
                createdAt: DateTime.UtcNow
            );
            users.Add(user);
        }
        dbContext.Set<User>().AddRange(users);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task SeedHouseholdsAsync(CancellationToken cancellationToken = default)
    {
        Role? householdHeadRole = await dbContext.Set<Role>()
            .FindAsync([Role.HouseholdHead.Name], cancellationToken);
        Role? householdMemberRole = await dbContext.Set<Role>()
            .FindAsync([Role.HouseholdMember.Name], cancellationToken);
        if (householdHeadRole == null || householdMemberRole == null)
        {
            throw new InvalidOperationException("Required roles are missing in the database.");
        }
        List<User> users = await dbContext.Set<User>().Include(u => u.Roles).Where(u => !u.Roles.Any() && u.HouseholdId == null).ToListAsync(cancellationToken);
        if (users.Count < 50)
        {
            await SeedRandomUsersAsync(cancellationToken);
            users = await dbContext.Set<User>().Include(u => u.Roles).Where(u => !u.Roles.Any() && u.HouseholdId == null).ToListAsync(cancellationToken);
        }
        var randomHouseholdHeads = users.OrderBy(_ => Random.Shared.Next()).Take(10).ToList();
        var theRest = users.Except(randomHouseholdHeads).ToList();
        string[] names = new[]
        {
            "Home", "Family", "Household", "People", "Crew", "Squad", "Gang", "Nest", "Clan", "Tribe"
        };
        var households = new List<Household>();
        foreach (string name in names)
        {
            User householdHead = randomHouseholdHeads[randomHouseholdHeads.Count - 1];
            var household = Household.Create(
                name: $"{householdHead.FirstName}'s {name}",
                createdOn: DateTime.UtcNow,
                userId: householdHead.Id
            );
            var householdMembers = theRest.Take(Random.Shared.Next(theRest.Count / 10)).ToList();
            theRest = theRest.Except(householdMembers).ToList();
            householdHead.SetHouseholdId(household.Id);
            householdHead.SetRoles(new[] { householdMemberRole });
            foreach (User member in householdMembers)
            {
                member.JoinHousehold(household.Id);
                member.SetRoles(new[] {householdMemberRole });
            }
            households.Add(household);
            
        }
        dbContext.Set<User>().UpdateRange(users);
        dbContext.Set<Household>().AddRange(households);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task SeedFoodItemsAsync(CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Set<FoodReference>().AnyAsync(cancellationToken))
        {
            throw new InvalidOperationException("Food references must be imported before seeding food items.");
        }
        List<Household> households = await dbContext.Set<Household>().ToListAsync(cancellationToken);
        if (households.Count == 0)
        {
            await SeedHouseholdsAsync(cancellationToken);
            households = await dbContext.Set<Household>().ToListAsync(cancellationToken);
        }
        foreach (Household household in households)
        {
            User? user = await dbContext.Set<User>()
                .FirstOrDefaultAsync(u => u.HouseholdId == household.Id, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"No user found for household {household.Id}");
            }
            var foodItems = new List<FoodItem>();
            List<Storage> storages = await dbContext.Set<Storage>()
                .Where(s => s.HouseholdId == household.Id)
                .ToListAsync(cancellationToken);
            if (storages.Count == 0)
            {
                await SeedStoragesAsync(household.Id, cancellationToken);
                storages = await dbContext.Set<Storage>()
                    .Where(s => s.HouseholdId == household.Id)
                    .ToListAsync(cancellationToken);
            }
            foreach (Storage storage in storages)
            {
                List<Compartment> compartments = await dbContext.Set<Compartment>()
                    .Where(c => c.StorageId == storage.Id)
                    .ToListAsync(cancellationToken);
                if (compartments.Count == 0)
                {
                    await SeedCompartment(storage, cancellationToken);
                    compartments = await dbContext.Set<Compartment>()
                        .Where(c => c.StorageId == storage.Id)
                        .ToListAsync(cancellationToken);
                }
                foreach(Compartment compartment in compartments)
                {
                    List<FoodReference> foodReferences = await dbContext.Set<FoodReference>()
                        .OrderBy(_ => Random.Shared.Next())
                        .Take(5)
                        .ToListAsync(cancellationToken);
                    foreach (FoodReference foodReference in foodReferences)
                    {
                        Unit? unit = await dbContext.Set<Unit>().OrderBy(_ => Random.Shared.Next())
                            .FirstOrDefaultAsync(u => u.Type == foodReference.UnitType, cancellationToken);
                        if (unit == null)
                        {
                            throw new InvalidOperationException($"No unit found for unit type {foodReference.UnitType}");
                        }
                        var foodItem = FoodItem.Create(
                            foodReference.Id,
                            compartment.Id,
                            household.Id,
                            foodReference.Name,
                            foodReference.ImageUrl,
                            quantity: Random.Shared.Next(1, 100),
                            unit.Id,
                            expirationDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(Random.Shared.Next(-365, 365))),
                            null,
                            user.Id
                        );
                        foodItems.Add(foodItem);
                    }
                }
            }
            
            dbContext.Set<FoodItem>().AddRange(foodItems);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        
    }
    public async Task SeedStoragesAsync(Guid householdId,  CancellationToken cancellationToken = default)
    {
        var pantry = Storage.AutoCreate("Pantry", householdId, StorageType.Pantry);
        var fridge = Storage.AutoCreate("Fridge", householdId, StorageType.Fridge);
        var freezer = Storage.AutoCreate("Freezer", householdId, StorageType.Freezer);
        dbContext.Set<Storage>().AddRange(pantry, fridge, freezer);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task SeedCompartment(Storage storage, CancellationToken cancellationToken = default)
    {
        var upper = Compartment.AutoCreate("Upper", storage.Id, storage.HouseholdId);
        var middle = Compartment.AutoCreate("Middle", storage.Id, storage.HouseholdId);
        var lower = Compartment.AutoCreate("Lower", storage.Id, storage.HouseholdId);
        dbContext.Set<Compartment>().AddRange(upper, middle, lower);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
