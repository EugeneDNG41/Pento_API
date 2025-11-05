using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.BlogPosts;
using Pento.Domain.Shared;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;
internal sealed class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.ToTable("blog_posts");
        builder.HasKey(bp => bp.Id);
        builder.Property(bp => bp.Content).HasMaxLength(2000).HasConversion(content => content.Value, value => Content.Create(value));

        builder.HasOne<User>().WithMany().HasForeignKey(bp => bp.UserId);
        builder.HasQueryFilter(c => !c.IsDeleted);

    }
}
