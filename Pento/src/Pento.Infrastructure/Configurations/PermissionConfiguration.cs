using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Permissions;
using Pento.Domain.Roles;

namespace Pento.Infrastructure.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(p => p.Code);

        builder.Property(p => p.Code).HasMaxLength(100);

        builder.Property(p => p.Name).HasMaxLength(100);
        builder.Property(p => p.Description).HasMaxLength(500);
        
        builder.HasData(
            Permission.UserGeneral,
            Permission.ViewRecipes,
            Permission.AddRecipes,
            Permission.UpdateRecipes,
            Permission.DeleteRecipes,
            Permission.ViewGiveaways,

            Permission.DeleteGiveaways,
            Permission.CreateGiveaways,
            Permission.UpdateGiveaways,

            Permission.GetUsers,
            Permission.ManageUsers,
            Permission.DeleteUsers,
            Permission.ViewHouseholds,
            Permission.ManageHouseholds,
            Permission.ViewRoles,
            Permission.ManageRoles,
            Permission.ViewPermissions,
            Permission.ManageGiveaways,
            Permission.ManageRecipes,
            Permission.ManageFoodReferences,

            Permission.ViewHousehold,
            Permission.ManageHousehold,
            Permission.ManageMembers,
            Permission.RemoveMembers,
            Permission.ViewStorages,
            Permission.AddStorages,
            Permission.UpdateStorages,
            Permission.DeleteStorage,
            Permission.ViewCompartments,
            Permission.AddCompartments,
            Permission.UpdateCompartments,
            Permission.DeleteCompartments,
            Permission.ViewFoodItems,
            Permission.AddFoodItems,
            Permission.UpdateFoodItems,
            Permission.DeleteFoodItems,
            Permission.ViewGroceries,
            Permission.AddGroceries,
            Permission.UpdateGroceries,
            Permission.DeleteGroceries,
            Permission.ViewMealPlans,
            Permission.AddMealPlans,
            Permission.UpdateMealPlans,
            Permission.DeleteMealPlans,

            Permission.ManagePayments,
            Permission.ManageSubscriptions,
            Permission.ManageMilestones
            );

        builder
            .HasMany<Role>()
            .WithMany()
            .UsingEntity(joinBuilder =>
            {
                joinBuilder.ToTable("role_permissions");

                joinBuilder.HasData(
                    CreateRolePermission(Role.User, Permission.UserGeneral),
                    CreateRolePermission(Role.User, Permission.ViewRecipes),
                    CreateRolePermission(Role.User, Permission.AddRecipes),
                    CreateRolePermission(Role.User, Permission.UpdateRecipes),
                    CreateRolePermission(Role.User, Permission.DeleteRecipes),
                    CreateRolePermission(Role.User, Permission.ViewGiveaways),
                    // Admin permissions
                    CreateRolePermission(Role.Administrator, Permission.GetUsers),
                    CreateRolePermission(Role.Administrator, Permission.ManageUsers),
                    CreateRolePermission(Role.Administrator, Permission.DeleteUsers),
                    CreateRolePermission(Role.Administrator, Permission.ViewHouseholds),
                    CreateRolePermission(Role.Administrator, Permission.ManageHouseholds),
                    CreateRolePermission(Role.Administrator, Permission.ViewRoles),
                    CreateRolePermission(Role.Administrator, Permission.ManageRoles),
                    CreateRolePermission(Role.Administrator, Permission.ViewPermissions),
                    CreateRolePermission(Role.Administrator, Permission.ManageGiveaways),
                    CreateRolePermission(Role.Administrator, Permission.ManageRecipes),
                    CreateRolePermission(Role.Administrator, Permission.ManageFoodReferences),
                    CreateRolePermission(Role.Administrator, Permission.ManagePayments),
                    CreateRolePermission(Role.Administrator, Permission.ManageSubscriptions),
                    CreateRolePermission(Role.Administrator, Permission.ManageMilestones),
                    // Household permissions
                    CreateRolePermission(Role.HouseholdHead, Permission.ViewHousehold),
                    CreateRolePermission(Role.HouseholdHead, Permission.ManageHousehold),
                    CreateRolePermission(Role.HouseholdHead, Permission.ManageMembers),
                    CreateRolePermission(Role.HouseholdHead, Permission.RemoveMembers),
                    CreateRolePermission(Role.HouseholdHead, Permission.ViewStorages),
                    CreateRolePermission(Role.HouseholdHead, Permission.AddStorages),
                    CreateRolePermission(Role.HouseholdHead, Permission.UpdateStorages),
                    CreateRolePermission(Role.HouseholdHead, Permission.DeleteStorage),
                    CreateRolePermission(Role.HouseholdHead, Permission.ViewCompartments),
                    CreateRolePermission(Role.HouseholdHead, Permission.AddCompartments),
                    CreateRolePermission(Role.HouseholdHead, Permission.UpdateCompartments),
                    CreateRolePermission(Role.HouseholdHead, Permission.DeleteCompartments),
                    CreateRolePermission(Role.HouseholdHead, Permission.ViewFoodItems),
                    CreateRolePermission(Role.HouseholdHead, Permission.AddFoodItems),
                    CreateRolePermission(Role.HouseholdHead, Permission.UpdateFoodItems),
                    CreateRolePermission(Role.HouseholdHead, Permission.DeleteFoodItems),
                    CreateRolePermission(Role.HouseholdHead, Permission.ViewGroceries),
                    CreateRolePermission(Role.HouseholdHead, Permission.AddGroceries),
                    CreateRolePermission(Role.HouseholdHead, Permission.UpdateGroceries),
                    CreateRolePermission(Role.HouseholdHead, Permission.DeleteGroceries),
                    CreateRolePermission(Role.HouseholdHead, Permission.ViewMealPlans),
                    CreateRolePermission(Role.HouseholdHead, Permission.AddMealPlans),
                    CreateRolePermission(Role.HouseholdHead, Permission.UpdateMealPlans),
                    CreateRolePermission(Role.HouseholdHead, Permission.DeleteMealPlans),
                    CreateRolePermission(Role.HouseholdHead, Permission.CreateGiveaways),
                    CreateRolePermission(Role.HouseholdHead, Permission.UpdateGiveaways),
                    CreateRolePermission(Role.HouseholdHead, Permission.DeleteGiveaways),

                    CreateRolePermission(Role.PantryManager, Permission.ViewHousehold),
                    CreateRolePermission(Role.PantryManager, Permission.ViewStorages),
                    CreateRolePermission(Role.PantryManager, Permission.AddStorages),
                    CreateRolePermission(Role.PantryManager, Permission.UpdateStorages),
                    CreateRolePermission(Role.PantryManager, Permission.DeleteStorage),
                    CreateRolePermission(Role.PantryManager, Permission.ViewCompartments),
                    CreateRolePermission(Role.PantryManager, Permission.AddCompartments),
                    CreateRolePermission(Role.PantryManager, Permission.UpdateCompartments),
                    CreateRolePermission(Role.PantryManager, Permission.DeleteCompartments),
                    CreateRolePermission(Role.PantryManager, Permission.ViewFoodItems),
                    CreateRolePermission(Role.PantryManager, Permission.AddFoodItems),
                    CreateRolePermission(Role.PantryManager, Permission.UpdateFoodItems),
                    CreateRolePermission(Role.PantryManager, Permission.DeleteFoodItems),
                    CreateRolePermission(Role.PantryManager, Permission.ViewGroceries),
                    CreateRolePermission(Role.PantryManager, Permission.AddGroceries),
                    CreateRolePermission(Role.PantryManager, Permission.UpdateGroceries),
                    CreateRolePermission(Role.PantryManager, Permission.ViewMealPlans),
                    CreateRolePermission(Role.PantryManager, Permission.AddMealPlans),
                    CreateRolePermission(Role.PantryManager, Permission.UpdateMealPlans),
                    CreateRolePermission(Role.PantryManager, Permission.CreateGiveaways),
                    CreateRolePermission(Role.PantryManager, Permission.UpdateGiveaways),
                    CreateRolePermission(Role.PantryManager, Permission.DeleteGiveaways),
                  
                    CreateRolePermission(Role.MealPlanner, Permission.ViewHousehold),
                    CreateRolePermission(Role.MealPlanner, Permission.ViewStorages),
                    CreateRolePermission(Role.MealPlanner, Permission.ViewCompartments),
                    CreateRolePermission(Role.MealPlanner, Permission.ViewFoodItems),
                    CreateRolePermission(Role.MealPlanner, Permission.AddFoodItems),
                    CreateRolePermission(Role.MealPlanner, Permission.UpdateFoodItems),
                    CreateRolePermission(Role.MealPlanner, Permission.ViewGroceries),
                    CreateRolePermission(Role.MealPlanner, Permission.AddGroceries),
                    CreateRolePermission(Role.MealPlanner, Permission.UpdateGroceries),
                    CreateRolePermission(Role.MealPlanner, Permission.ViewMealPlans),
                    CreateRolePermission(Role.MealPlanner, Permission.AddMealPlans),
                    CreateRolePermission(Role.MealPlanner, Permission.UpdateMealPlans),
                    CreateRolePermission(Role.MealPlanner, Permission.DeleteMealPlans),

                    CreateRolePermission(Role.GroceryShopper, Permission.ViewHousehold),
                    CreateRolePermission(Role.GroceryShopper, Permission.ViewStorages),
                    CreateRolePermission(Role.GroceryShopper, Permission.ViewCompartments),
                    CreateRolePermission(Role.GroceryShopper, Permission.ViewFoodItems),
                    CreateRolePermission(Role.GroceryShopper, Permission.AddFoodItems),
                    CreateRolePermission(Role.GroceryShopper, Permission.ViewGroceries),
                    CreateRolePermission(Role.GroceryShopper, Permission.AddGroceries),
                    CreateRolePermission(Role.GroceryShopper, Permission.UpdateGroceries),
                    CreateRolePermission(Role.GroceryShopper, Permission.DeleteGroceries),
                    CreateRolePermission(Role.GroceryShopper, Permission.ViewMealPlans));

            });
    }

    private static object CreateRolePermission(Role role, Permission permission)
    {
        return new
        {
            RoleName = role.Name,
            PermissionCode = permission.Code
        };
    }
}
