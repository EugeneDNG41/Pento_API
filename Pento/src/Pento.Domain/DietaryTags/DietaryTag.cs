using Pento.Domain.Abstractions;

namespace Pento.Domain.DietaryTags;

public sealed class DietaryTag : Entity
{
    private DietaryTag() { }

    public DietaryTag(Guid id, string name, string? description = null)
        : base(id)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public static DietaryTag Create(string name, string? description = null)
    {
        return new DietaryTag(Guid.CreateVersion7(), name, description);
    }
}
