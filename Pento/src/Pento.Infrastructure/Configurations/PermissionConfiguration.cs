using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(p => p.Code);

        builder.Property(p => p.Code).HasMaxLength(100);

        builder.HasData(
            Permission.GetUser,
            Permission.ModifyUser,
            Permission.ViewHousehold,
            Permission.UpdateHousehold,
            Permission.TransferOwner,
            Permission.ViewMembers,
            Permission.InviteMember,
            Permission.UpdateMember,
            Permission.RemoveMember,
            Permission.ViewStorage,
            Permission.ManageStorage,
            Permission.ViewPantry,
            Permission.AddPantryItem,
            Permission.UpdatePantry,
            Permission.ConsumePantry,
            Permission.DiscardPantry,
            Permission.ViewGroceries,
            Permission.ManageGroceries,
            Permission.ViewMealPlans,
            Permission.ManageMealPlans,
            Permission.ViewRecipes,
            Permission.ManageRecipes,
            Permission.ViewGiveaways,
            Permission.ManageGiveaways,
            Permission.ManagePermissions,
            Permission.ManageOverrides
        );

        builder
            .HasMany<Role>()
            .WithMany()
            .UsingEntity(joinBuilder =>
            {
                joinBuilder.ToTable("role_permissions");

                joinBuilder.HasData(
                    // Administrator: full access
                    CreateRolePermission(Role.Administrator, Permission.GetUser),
                    CreateRolePermission(Role.Administrator, Permission.ModifyUser),
                    CreateRolePermission(Role.Administrator, Permission.ViewHousehold),
                    CreateRolePermission(Role.Administrator, Permission.UpdateHousehold),
                    CreateRolePermission(Role.Administrator, Permission.TransferOwner),
                    CreateRolePermission(Role.Administrator, Permission.ViewMembers),
                    CreateRolePermission(Role.Administrator, Permission.InviteMember),
                    CreateRolePermission(Role.Administrator, Permission.UpdateMember),
                    CreateRolePermission(Role.Administrator, Permission.RemoveMember),
                    CreateRolePermission(Role.Administrator, Permission.ViewStorage),
                    CreateRolePermission(Role.Administrator, Permission.ManageStorage),
                    CreateRolePermission(Role.Administrator, Permission.ViewPantry),
                    CreateRolePermission(Role.Administrator, Permission.AddPantryItem),
                    CreateRolePermission(Role.Administrator, Permission.UpdatePantry),
                    CreateRolePermission(Role.Administrator, Permission.ConsumePantry),
                    CreateRolePermission(Role.Administrator, Permission.DiscardPantry),
                    CreateRolePermission(Role.Administrator, Permission.ViewGroceries),
                    CreateRolePermission(Role.Administrator, Permission.ManageGroceries),
                    CreateRolePermission(Role.Administrator, Permission.ViewMealPlans),
                    CreateRolePermission(Role.Administrator, Permission.ManageMealPlans),
                    CreateRolePermission(Role.Administrator, Permission.ViewRecipes),
                    CreateRolePermission(Role.Administrator, Permission.ManageRecipes),
                    CreateRolePermission(Role.Administrator, Permission.ViewGiveaways),
                    CreateRolePermission(Role.Administrator, Permission.ManageGiveaways),
                    CreateRolePermission(Role.Administrator, Permission.ManagePermissions),
                    CreateRolePermission(Role.Administrator, Permission.ManageOverrides),

                    // HouseholdAdmin
                    CreateRolePermission(Role.HouseholdAdmin, Permission.ViewHousehold),
                    CreateRolePermission(Role.HouseholdAdmin, Permission.UpdateHousehold),
                    CreateRolePermission(Role.HouseholdAdmin, Permission.TransferOwner),
                    CreateRolePermission(Role.HouseholdAdmin, Permission.ViewMembers),
                    CreateRolePermission(Role.HouseholdAdmin, Permission.InviteMember),
                    CreateRolePermission(Role.HouseholdAdmin, Permission.UpdateMember),
                    CreateRolePermission(Role.HouseholdAdmin, Permission.RemoveMember),
                    CreateRolePermission(Role.HouseholdAdmin, Permission.ManagePermissions),
                    CreateRolePermission(Role.HouseholdAdmin, Permission.ManageOverrides),

                    // PowerMember
                    CreateRolePermission(Role.PowerMember, Permission.ViewHousehold),
                    CreateRolePermission(Role.PowerMember, Permission.UpdateHousehold),
                    CreateRolePermission(Role.PowerMember, Permission.ViewStorage),
                    CreateRolePermission(Role.PowerMember, Permission.ManageStorage),
                    CreateRolePermission(Role.PowerMember, Permission.ViewPantry),
                    CreateRolePermission(Role.PowerMember, Permission.AddPantryItem),
                    CreateRolePermission(Role.PowerMember, Permission.UpdatePantry),
                    CreateRolePermission(Role.PowerMember, Permission.ConsumePantry),
                    CreateRolePermission(Role.PowerMember, Permission.DiscardPantry),
                    CreateRolePermission(Role.PowerMember, Permission.ViewGroceries),
                    CreateRolePermission(Role.PowerMember, Permission.ManageGroceries),
                    CreateRolePermission(Role.PowerMember, Permission.ViewMealPlans),
                    CreateRolePermission(Role.PowerMember, Permission.ManageMealPlans),
                    CreateRolePermission(Role.PowerMember, Permission.ViewRecipes),
                    CreateRolePermission(Role.PowerMember, Permission.ManageRecipes),
                    CreateRolePermission(Role.PowerMember, Permission.ViewGiveaways),
                    CreateRolePermission(Role.PowerMember, Permission.ManageGiveaways),

                    // GroceryRunner
                    CreateRolePermission(Role.GroceryRunner, Permission.ViewGroceries),
                    CreateRolePermission(Role.GroceryRunner, Permission.ManageGroceries),

                    // MealPlanner
                    CreateRolePermission(Role.MealPlanner, Permission.ViewMealPlans),
                    CreateRolePermission(Role.MealPlanner, Permission.ManageMealPlans),
                    CreateRolePermission(Role.MealPlanner, Permission.ViewRecipes),
                    CreateRolePermission(Role.MealPlanner, Permission.ManageRecipes),

                    // StorageManager
                    CreateRolePermission(Role.StorageManager, Permission.ViewStorage),
                    CreateRolePermission(Role.StorageManager, Permission.ManageStorage),
                    CreateRolePermission(Role.StorageManager, Permission.ViewPantry),
                    CreateRolePermission(Role.StorageManager, Permission.AddPantryItem),
                    CreateRolePermission(Role.StorageManager, Permission.UpdatePantry),
                    CreateRolePermission(Role.StorageManager, Permission.ConsumePantry),
                    CreateRolePermission(Role.StorageManager, Permission.DiscardPantry),

                    // BasicMember (view-only)
                    CreateRolePermission(Role.BasicMember, Permission.ViewHousehold),
                    CreateRolePermission(Role.BasicMember, Permission.ViewMembers),
                    CreateRolePermission(Role.BasicMember, Permission.ViewStorage),
                    CreateRolePermission(Role.BasicMember, Permission.ViewPantry),
                    CreateRolePermission(Role.BasicMember, Permission.ViewGroceries),
                    CreateRolePermission(Role.BasicMember, Permission.ViewMealPlans),
                    CreateRolePermission(Role.BasicMember, Permission.ViewRecipes),
                    CreateRolePermission(Role.BasicMember, Permission.ViewGiveaways)
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
