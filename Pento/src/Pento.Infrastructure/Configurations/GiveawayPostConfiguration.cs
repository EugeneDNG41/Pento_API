using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.GiveawayPosts;
using Pento.Domain.FoodItems;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class GiveawayPostConfiguration : IEntityTypeConfiguration<GiveawayPost>
{
    public void Configure(EntityTypeBuilder<GiveawayPost> builder)
    {
        builder.ToTable("giveaway_posts");
        builder.HasOne<User>().WithMany().HasForeignKey(c => c.UserId);
        builder.HasQueryFilter(x => !x.IsArchived && !x.IsDeleted);

    }
}
