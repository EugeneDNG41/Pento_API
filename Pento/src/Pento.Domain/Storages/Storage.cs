using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Storages;

public sealed class Storage : Entity
{
    public Storage(Guid id, string name, Guid householdId, StorageType type, string? notes) : base(id)
    {
        Name = name;
        HouseholdId = householdId;
        Type = type;
        Notes = notes;
    }
    private Storage() { }
    public string Name { get; private set; }
    public Guid HouseholdId { get; private set; }
    public StorageType Type { get; private set; }
    public string? Notes { get; private set; }
    public static Storage Create(string name, Guid householdId, StorageType type, string? notes)
    {
        return new Storage(
            Guid.CreateVersion7(),
            name,
            householdId,
            type,
            notes);
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
