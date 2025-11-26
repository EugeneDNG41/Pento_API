using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Exceptions;
using Pento.Domain.Abstractions;
using Pento.Domain.DietaryTags;
using Pento.Domain.FoodDietaryTags;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;
using Pento.Infrastructure.Outbox;

namespace Pento.Infrastructure;

public sealed class ApplicationDbContext(DbContextOptions options)  : DbContext(options), IUnitOfWork
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
    public DbSet<FoodReference> FoodReferences { get; set; } = null!;
    public DbSet<Unit> Units { get; set; } = null!;
    
    public DbSet<DietaryTag> DietaryTags { get; set; } = null!;
    public DbSet<FoodDietaryTag> FoodDietaryTags { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            AddDomainEventsAsOutboxMessages();

            int result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occurred.", ex);
        }
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                IReadOnlyList<IDomainEvent> events = entity.GetDomainEvents();

                entity.ClearDomainEvents();

                return events;

            }).ToList();
        var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                domainEvent.Timestamp,
                domainEvent.GetType().AssemblyQualifiedName!,
                JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
            .ToList();

        AddRange(outboxMessages);
    }
}
