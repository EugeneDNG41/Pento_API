using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Domain.BlogPosts;
using Pento.Domain.Comments;
using Pento.Domain.Shared;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Configurations;

internal sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");
        builder.HasKey(x => x.Id);
        builder.Property(c => c.Content).HasMaxLength(255);

        builder.HasOne<User>().WithMany().HasForeignKey(c => c.UserId);
        builder.HasOne<BlogPost>().WithMany().HasForeignKey(c => c.BlogPostId);
        builder.HasQueryFilter(c => !c.IsDeleted && !c.IsArchived);

    }
}
