using Pento.Domain.Abstractions;

namespace Pento.Domain.Subscriptions;

public sealed class Subscription : Entity
{
    private Subscription() { }
    public Subscription(Guid id, string name, string description, bool isActive) : base(id)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
    }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }

    public static Subscription Create(string name, string description, bool isActive)
        => new(Guid.CreateVersion7(), name, description, isActive);
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

