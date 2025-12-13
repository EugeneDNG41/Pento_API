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

                    CreateRolePermission(Role.HouseholdMember, Permission.ViewHousehold),
                    CreateRolePermission(Role.HouseholdMember, Permission.ManageHousehold),
                    CreateRolePermission(Role.HouseholdMember, Permission.ViewStorages),
                    CreateRolePermission(Role.HouseholdMember, Permission.AddStorages),
                    CreateRolePermission(Role.HouseholdMember, Permission.UpdateStorages),
                    CreateRolePermission(Role.HouseholdMember, Permission.DeleteStorage),
                    CreateRolePermission(Role.HouseholdMember, Permission.ViewCompartments),
                    CreateRolePermission(Role.HouseholdMember, Permission.AddCompartments),
                    CreateRolePermission(Role.HouseholdMember, Permission.UpdateCompartments),
                    CreateRolePermission(Role.HouseholdMember, Permission.DeleteCompartments),
                    CreateRolePermission(Role.HouseholdMember, Permission.ViewFoodItems),
                    CreateRolePermission(Role.HouseholdMember, Permission.AddFoodItems),
                    CreateRolePermission(Role.HouseholdMember, Permission.UpdateFoodItems),
                    CreateRolePermission(Role.HouseholdMember, Permission.DeleteFoodItems),
                    CreateRolePermission(Role.HouseholdMember, Permission.ViewGroceries),
                    CreateRolePermission(Role.HouseholdMember, Permission.AddGroceries),
                    CreateRolePermission(Role.HouseholdMember, Permission.UpdateGroceries),
                    CreateRolePermission(Role.HouseholdMember, Permission.DeleteGroceries),
                    CreateRolePermission(Role.HouseholdMember, Permission.ViewMealPlans),
                    CreateRolePermission(Role.HouseholdMember, Permission.AddMealPlans),
                    CreateRolePermission(Role.HouseholdMember, Permission.UpdateMealPlans),
                    CreateRolePermission(Role.HouseholdMember, Permission.DeleteMealPlans),
                    CreateRolePermission(Role.HouseholdMember, Permission.CreateGiveaways),
                    CreateRolePermission(Role.HouseholdMember, Permission.UpdateGiveaways),
                    CreateRolePermission(Role.HouseholdMember, Permission.DeleteGiveaways)

                    );

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
