using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.Milestones;

public sealed class Milestone : Entity
{
    private Milestone() { }
    public Milestone(Guid id, string name, string description, Uri? iconUrl, bool isActive) : base(id)
    {
        Name = name;
        Description = description;
        IconUrl = iconUrl;
        IsActive = isActive;
    }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Uri? IconUrl { get; private set; }
    public bool IsActive { get; private set; }
    public static Milestone Create(string name, string description, Uri? iconUrl, bool isActive = false)
    {
        return new Milestone(Guid.CreateVersion7(), name, description, iconUrl, isActive);
    }
    public void UpdateDetails(string? name, string? description)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description;
        }
    }
    public void UpdateIconUrl(Uri? iconUrl)
    {
        if (IconUrl != iconUrl)
        {
            IconUrl = iconUrl;
        }
    }
    public void Enable()
    {
        IsActive = true;
        Raise(new MilestoneEnabledOrUpdatedDomainEvent(Id));
    }
    public void Disable()
    {
        IsActive = false;
    }
}
