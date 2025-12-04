using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Compartments;

public sealed class Compartment : Entity
{
    public Compartment(Guid id, string name, Guid storageId, Guid householdId, string? notes) : base(id)
    {
        Name = name;
        StorageId = storageId;
        HouseholdId = householdId;
        Notes = notes;
    }
    private Compartment() { }
    public string Name { get; private set; }
    public Guid StorageId { get; private set; }
    public Guid HouseholdId { get; private set; }
    public string? Notes { get; private set; }
    public static Compartment Create(string name, Guid storageId, Guid householdId, string? notes, Guid userId)
    {
        var compartment = new Compartment(Guid.CreateVersion7(), name, storageId, householdId, notes);
        compartment.Raise(new CompartmentCreatedDomainEvent(compartment.Id, userId));
        return compartment;
    }
    public void UpdateName(string name)
    {
        Name = name;
    }
    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }
}
public sealed class CompartmentCreatedDomainEvent(Guid compartmentId, Guid userId) : DomainEvent
{
    public Guid CompartmentId { get; } = compartmentId;
    public Guid UserId { get; } = userId;
}
