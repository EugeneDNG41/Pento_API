using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.Abstractions;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    protected Entity(Guid id) => Id = id;
    protected Entity() { }
    public Guid Id { get; init; }
    public bool IsDeleted { get; private set; }
    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();
    public void ClearDomainEvents() => _domainEvents.Clear();
    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void Delete() => IsDeleted = true;
}
