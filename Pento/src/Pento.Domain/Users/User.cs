using Pento.Domain.Abstractions;
using Pento.Domain.Roles;
using Pento.Domain.Users.Events;

namespace Pento.Domain.Users;

public sealed class User : Entity
{
    private User()
    {
    }

    public Guid? HouseholdId { get; private set; }
    public Uri? AvatarUrl { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string IdentityId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<Role> Roles => _roles.ToList();

    private readonly List<Role> _roles = [];

    public static User Create(string email, string firstName, string lastName, string identityId, DateTime createdAt)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            IdentityId = identityId,
            CreatedAt = createdAt
        };
        user.Raise(new UserCreatedDomainEvent(user.Id));
        user.Raise(new UserRegisteredDomainEvent(user.IdentityId));

        return user;
    }
    public void Update(string firstName, string lastName)
    {
        if (FirstName == firstName && LastName == lastName)
        {
            return;
        }

        FirstName = firstName;
        LastName = lastName;
    }
    public void SetHouseholdId(Guid? householdId)
    {
        if (HouseholdId.HasValue && HouseholdId != householdId)
        {
            Raise(new UserLeftHouseholdDomainEvent(HouseholdId.Value));
        }
        HouseholdId = householdId;
    }
    public void JoinHousehold(Guid householdId)
    {
        HouseholdId = householdId;
        Raise(new UserHouseholdJoinedDomainEvent(Id, householdId));
    }
    public void SetAvatarUrl(Uri? avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }
    public void SetRoles(IEnumerable<Role> setRoles)
    {
        var toAdd = setRoles.Where(sr => !_roles.Any(r => r.Name == sr.Name)).ToList();
        var toRemove = _roles.Where(r => !setRoles.Any(sr => sr.Name == r.Name)).ToList();
        foreach (Role? role in toAdd)
        {
            _roles.Add(role);
        }
        foreach (Role? role in toRemove)
        {
            _roles.Remove(role);
        }
    }
}
