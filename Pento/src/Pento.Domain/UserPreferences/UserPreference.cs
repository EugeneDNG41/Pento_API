using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.UserPreferences;
public sealed class UserPreference : Entity
{
    private UserPreference() { }

    public UserPreference(Guid id, Guid userId, Guid dietaryTagId)
        : base(id)
    {
        UserId = userId;
        DietaryTagId = dietaryTagId;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid UserId { get; private set; }
    public Guid DietaryTagId { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    public static UserPreference Create(Guid userId, Guid dietaryTagId)
    {
        return new UserPreference(Guid.CreateVersion7(), userId, dietaryTagId);
    }
}
