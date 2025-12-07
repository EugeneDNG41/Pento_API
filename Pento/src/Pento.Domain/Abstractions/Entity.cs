namespace Pento.Domain.Abstractions;

public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();
    public void ClearDomainEvents() => _domainEvents.Clear();
    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
public abstract class Entity : BaseEntity
{
    protected Entity(Guid id) => Id = id;
    protected Entity() { }
    public Guid Id { get; init; }
    public bool IsDeleted { get; private set; }
    public void Delete() => IsDeleted = true;
}
