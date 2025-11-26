using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.Subscriptions;

namespace Pento.Infrastructure.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(20);
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.HasQueryFilter(x => !x.IsArchived && !x.IsDeleted);
    }
}
