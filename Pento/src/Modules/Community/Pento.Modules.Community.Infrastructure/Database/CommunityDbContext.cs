using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pento.Modules.Community.Application.Abstractions.Data;
using Pento.Modules.Community.Domain.BlogPost;
using Pento.Modules.Community.Domain.Comment;
using Pento.Modules.Community.Domain.GiveawayClaim;
using Pento.Modules.Community.Domain.GiveawayPost;
namespace Pento.Modules.Community.Infrastructure.Database;

public sealed class CommunityDbContext(DbContextOptions<CommunityDbContext> options)
    : DbContext(options), IUnitOfWork
{
    internal DbSet<BlogPost> BlogPosts { get; set; }
    internal DbSet<Comment> Comments { get; set; }
    internal DbSet<GiveawayPost> GiveawayPosts { get; set; }
    internal DbSet<GiveawayClaim> GiveawayClaims { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Community);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommunityDbContext).Assembly);
    }
}
