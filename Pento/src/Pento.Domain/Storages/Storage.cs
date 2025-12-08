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
    public static Storage Create(string name, Guid householdId, StorageType type, string? notes, Guid userId)
    {
        var storage = new Storage(
            Guid.CreateVersion7(),
            name,
            householdId,
            type,
            notes);
        storage.Raise(new StorageCreatedDomainEvent(storage.Id, userId));
        return storage;
    }
    public static Storage AutoCreate(string name, Guid householdId, StorageType type, string? notes)
    {
        var storage = new Storage(
            Guid.CreateVersion7(),
            name,
            householdId,
            type,
            notes);
        return storage;
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
