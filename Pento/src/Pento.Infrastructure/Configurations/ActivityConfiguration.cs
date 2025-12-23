
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Activities;
using Pento.Domain.MilestoneRequirements;
using Pento.Domain.UserActivities;

namespace Pento.Infrastructure.Configurations;

internal sealed class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("activities");
        builder.HasKey(a => a.Code);
        builder.Property(a => a.Code).HasMaxLength(50);
        builder.Property(a => a.Name).HasMaxLength(100);
        builder.Property(a => a.Description).HasMaxLength(500);
        builder.HasMany<UserActivity>()
               .WithOne()
               .HasForeignKey(ua => ua.ActivityCode);
        builder.HasMany<MilestoneRequirement>()
               .WithOne()
               .HasForeignKey(mr => mr.ActivityCode);
        builder.HasData(
            Activity.CreateStorage,
            Activity.CreateCompartment,
            Activity.ConsumeFoodItem,
            Activity.IntakeFoodItem,
            Activity.DiscardFoodItem,
            Activity.CreateRecipe,
            Activity.CookRecipe,
            Activity.CookOtherRecipe,
            Activity.CreateMealPlan,
            Activity.FulfillMealPlan,
            Activity.CancelMealPlan,
            Activity.CreateGroceryList,
            Activity.CreateHousehold,
            Activity.JoinHousehold,
            Activity.HouseholdMemberJoined,
            Activity.TradeInFoodItem,
            Activity.TradeAwayFoodItem,
            Activity.CreateTradeOffer,
            Activity.CreateTradeRequest,
            Activity.CompleteTrade);
    }
}
