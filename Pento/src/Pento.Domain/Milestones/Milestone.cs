using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.Milestones;

public sealed class Milestone : Entity
{
    private Milestone() { }
    public Milestone(Guid id, string name, string description, bool isActive) : base(id)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
    }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public static Milestone Create(string name, string description, bool isActive = true)
    {
        return new Milestone(Guid.CreateVersion7(), name, description, isActive);
    }
    public void UpdateDetails(string? name, string? description, bool? isActive)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description;
        }
        if (isActive.HasValue && IsActive != isActive)
        {
            IsActive = isActive.Value;
        }
    }
}
