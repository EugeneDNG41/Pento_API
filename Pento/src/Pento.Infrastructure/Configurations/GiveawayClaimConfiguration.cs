using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.GiveawayClaims;
using Pento.Domain.GiveawayPosts;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class GiveawayClaimConfiguration : IEntityTypeConfiguration<GiveawayClaim>
{
    public void Configure(EntityTypeBuilder<GiveawayClaim> builder)
    {
        builder.ToTable("giveaway_claims");
        builder.HasOne<User>().WithMany().HasForeignKey(c => c.ClaimantId);
        builder.HasOne<GiveawayPost>().WithMany().HasForeignKey(c => c.GiveawayPostId);
        builder.HasQueryFilter(x => !x.IsDeleted);

    }
}
