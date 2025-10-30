using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Pento.Application.Abstractions.Authentication;
using Pento.Application.Abstractions.Clock;
using Pento.Application.Abstractions.Exceptions;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.GroceryLists;
using Pento.Domain.MealPlans;
using Pento.Domain.Storages;
using Pento.Infrastructure.Outbox;

namespace Pento.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUserContext _userContext;

    public ApplicationDbContext(DbContextOptions options, IDateTimeProvider dateTimeProvider, IUserContext userContext) : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
        _userContext = userContext;
    }
    private void TryEnsureDatabaseCreated()
    {
        try
        {
            if (Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreator)
            {
                if (!databaseCreator.CanConnect())
                {
                    databaseCreator.Create();
                }
                if (!databaseCreator.HasTables())
                {
                    databaseCreator.CreateTables();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database creation failed: {ex.Message}");
        }
    }
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        Guid? householdId = _userContext.HouseholdId;
        modelBuilder.Entity<Storage>().HasQueryFilter(s => householdId == null || s.HouseholdId == householdId);
        modelBuilder.Entity<MealPlan>().HasQueryFilter(s => householdId == null || s.HouseholdId == householdId);
        modelBuilder.Entity<GroceryList>().HasQueryFilter(s => householdId == null || s.HouseholdId == householdId);

        base.OnModelCreating(modelBuilder);
    }
    public DbSet<FoodReference> FoodReferences { get; set; } = null!;


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
        var outboxMessages = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                IReadOnlyList<IDomainEvent> domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                _dateTimeProvider.UtcNow,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
            .ToList();

        AddRange(outboxMessages);
    }
}
