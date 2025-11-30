using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.GroceryListAssignees;
using Pento.Domain.GroceryLists;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class GroceryListAssigneeConfiguration : IEntityTypeConfiguration<GroceryListAssignee>
{
    public void Configure(EntityTypeBuilder<GroceryListAssignee> builder)
    {
        builder.ToTable("grocery_list_assignees");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.GroceryListId)
            .HasColumnName("grocery_list_id")
            .IsRequired();

        builder.Property(x => x.HouseholdMemberId)
            .HasColumnName("household_member_id")
            .IsRequired();

        builder.Property(x => x.IsCompleted)
            .HasColumnName("is_completed")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.AssignedOnUtc)
            .HasColumnName("assigned_on_utc")
            .IsRequired();

        builder.HasIndex(x => new { x.GroceryListId, x.HouseholdMemberId })
            .IsUnique();

        builder.HasOne<GroceryList>()
            .WithMany()
            .HasForeignKey(x => x.GroceryListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.HouseholdMemberId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);

    }
}
